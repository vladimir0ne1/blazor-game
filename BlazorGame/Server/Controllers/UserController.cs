using BlazorGame.Server.Data;
using BlazorGame.Server.Services;
using BlazorGame.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thinktecture;

namespace BlazorGame.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly DataContext context;
        private readonly IUtilityService utilityService;

        public UserController(DataContext context, IUtilityService utilityService)
        {
            this.context = context;
            this.utilityService = utilityService;
        }

        [HttpGet("getbananas")]
        public async Task<IActionResult> GetBananas()
        {
            var user = await GetUser();

            return Ok(user.Bananas);
        }

        [HttpPut("addbananas")]
        public async Task<IActionResult> AddBananas([FromBody] int bananas)
        {
            var user = await GetUser();
            user.Bananas += bananas;
            await context.SaveChangesAsync();

            return Ok(user.Bananas);
        }

        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard()
        {
            var stats = await context.Users
                .Where(u => !u.IsDeleted && u.IsConfirmed)
                .Select(u => new UserStatistic
                {
                    Rank = EF.Functions.RowNumber(EF.Functions.OrderBy(u.Victories).ThenBy(u.Defeats)),
                    UserId = u.Id,
                    UserName = u.Username,
                    Battles = u.Battles,
                    Victories = u.Victories,
                    Defeats = u.Defeats,
                })
                .OrderByDescending(u => u.Victories)
                .ThenBy(u => u.Defeats)
                .ToListAsync();

            return Ok(stats);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            var user = await utilityService.GetUser();
            var battles = await context.Battles
                .Where(b => b.AttackerId == user.Id || b.OpponentId == user.Id)
                .Select(b => new BattleHistory
                {
                    BattleId = b.Id,
                    AttackerId = b.AttackerId,
                    OpponentId = b.OpponentId,
                    YouWon = b.WinnerId == user.Id,
                    AttackerName = b.Attacker.Username,
                    OpponentName = b.Opponent.Username,
                    RoundsFought = b.RoundsFought,
                    WinnerDamageDealt = b.WinnerDamage,
                    BattleDate = b.BattleDate,
                })
                .OrderByDescending(b => b.BattleDate)
                .ToListAsync();

            return Ok(battles);
        }
        private async Task<User> GetUser() => await utilityService.GetUser();
    }
}
