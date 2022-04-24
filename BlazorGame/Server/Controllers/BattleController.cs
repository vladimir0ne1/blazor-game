using BlazorGame.Server.Data;
using BlazorGame.Server.Services;
using BlazorGame.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorGame.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BattleController : ControllerBase
{
    private readonly DataContext context;
    private readonly IUtilityService utilityService;

    public BattleController(DataContext context, IUtilityService utilityService)
    {
        this.context = context;
        this.utilityService = utilityService;
    }

    [HttpPost]
    public async Task<IActionResult> StartBattle([FromBody] int opponentId)
    {
        var attacker = await utilityService.GetUser();
        var opponent = await context.Users.FindAsync(opponentId);
        if (opponent == null || opponent.IsDeleted)
        {
            return NotFound("Opponent not available");
        }

        var result = new BattleResult();

        await Fight(attacker, opponent, result);

        return Ok(result);
    }

    private async Task Fight(User attacker, User opponent, BattleResult result)
    {
        var attackerArmy = await context.UserUnits
            .Where(u => u.UserId == attacker.Id && u.HitPoints > 0)
            .Include(u => u.Unit)
            .ToListAsync();

       var opponentArmy = await context.UserUnits
            .Where(u => u.UserId == opponent.Id && u.HitPoints > 0)
            .Include(u => u.Unit)
            .ToListAsync();

       var attackerDamageSum = 0;
       var opponentDamageSum = 0;
       var currentRound = 0;

       while (attackerArmy.Count > 0 && opponentArmy.Count > 0)
       {
           currentRound++;

           if (currentRound % 2 != 0)
           {
               attackerDamageSum += FightRound(attacker, opponent, attackerArmy, opponentArmy, result);
           }
           else
           {
               opponentDamageSum += FightRound(opponent, attacker, opponentArmy, attackerArmy, result);
           }
       }

       result.IsVictory = opponentArmy.Count == 0;
       result.RoundsFought = currentRound;

       if (result.RoundsFought > 0)
       {
           await FinishFight(attacker, opponent, result, attackerDamageSum, opponentDamageSum);
       }
    }

    private int FightRound(
        User attacker,
        User opponent,
        List<UserUnit> attackerArmy,
        List<UserUnit> opponentArmy,
        BattleResult result)
    {
        int randomAttackerIndex = new Random().Next(attackerArmy.Count);
        int randomOpponentIndex = new Random().Next(opponentArmy.Count);

        var randomAttacker = attackerArmy[randomAttackerIndex];
        var randomOpponent = opponentArmy[randomOpponentIndex];

        var damage =
            new Random().Next(randomAttacker.Unit.Attack) - new Random().Next(randomOpponent.Unit.Defense);

        if (damage < 0)
        {
            damage = 0;
        }

        if (damage <= randomOpponent.HitPoints)
        {
            randomOpponent.HitPoints -= damage;
            result.Log.Add(
                $"{attacker.Username}'s {randomAttacker.Unit.Name} attacks " +
                $"{opponent.Username}'s {randomOpponent.Unit.Name} with {damage} damage.");
            return damage;
        }
        else
        {
            damage = randomOpponent.HitPoints;
            randomOpponent.HitPoints = 0;
            opponentArmy.Remove(randomOpponent);
            result.Log.Add(
                $"{attacker.Username}'s {randomAttacker.Unit.Name} kills " +
                $"{opponent.Username}'s {randomOpponent.Unit.Name}!");
            return damage;
        }
    }

    private async Task FinishFight(
        User attacker,
        User opponent,
        BattleResult result,
        int attackerDamageSum,
        int opponentDamageSum)
    {
        result.AttackerDamageSum = attackerDamageSum;
        result.OpponentDamageSum = opponentDamageSum;
        attacker.Battles++;
        opponent.Battles++;

        if (result.IsVictory)
        {
            attacker.Victories++;
            opponent.Defeats++;
            attacker.Bananas += opponentDamageSum;
            opponent.Bananas += attackerDamageSum * 10;
        }
        else
        {
            attacker.Defeats++;
            opponent.Victories++;
            attacker.Bananas += opponentDamageSum * 10;
            opponent.Bananas += attackerDamageSum;
        }

        StoreBattleHistory(attacker, opponent, result);

        await context.SaveChangesAsync();
    }

    private void StoreBattleHistory(User attacker, User opponent, BattleResult result)
    {
        var battle = new Battle
        {
            Attacker = attacker,
            Opponent = opponent,
            RoundsFought = result.RoundsFought,
            WinnerDamage = result.IsVictory ? result.AttackerDamageSum : result.OpponentDamageSum,
            Winner = result.IsVictory ? attacker : opponent
        };

        context.Battles.Add(battle);
    }
}
