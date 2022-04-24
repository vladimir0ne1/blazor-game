using BlazorGame.Server.Data;
using BlazorGame.Server.Services;
using BlazorGame.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorGame.Server.Controllers
{
    public class UserUnitResponse
    {
        public int UnitId { get; set; }
        public int HitPoints { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserUnitController : ControllerBase
    {
        private readonly DataContext context;
        private readonly IUtilityService utilityService;

        public UserUnitController(DataContext context, IUtilityService utilityService)
        {
            this.context = context;
            this.utilityService = utilityService;
        }

        [HttpPost]
        public async Task<IActionResult> BuildUserUnit([FromBody] int unitId)
        {
            var unit = await context.Units.FirstOrDefaultAsync<Unit>(u => u.Id == unitId);
            var user = await utilityService.GetUser();
            if (user.Bananas < unit.BananaCost)
            {
                return BadRequest("Not enough bananas!");
            }

            user.Bananas -= unit.BananaCost;
            var newUserUnit = new UserUnit
            {
                UnitId = unit.Id,
                UserId = user.Id,
                HitPoints = unit.HitPoints
            };

            context.UserUnits.Add(newUserUnit);

            await context.SaveChangesAsync();

            return Ok(newUserUnit);
        }

        [HttpPost("revive")]
        public async Task<IActionResult> ReviveArmy()
        {
            var user = await utilityService.GetUser();
            var userUnits = await context.UserUnits
                .Where(u => u.UserId == user.Id)
                .Include(u => u.Unit)
                .ToListAsync();

            int bananaCost = 1000;

            if (user.Bananas < bananaCost)
            {
                return BadRequest("Not enough bananas! You need 1000 bananas to revive your army.");
            }

            bool armyAlreadyAlive = true;

            foreach (var userUnit in userUnits)
            {
                if (userUnit.HitPoints <= 0)
                {
                    armyAlreadyAlive = false;
                    userUnit.HitPoints = new Random().Next(0, userUnit.Unit.HitPoints);
                }
            }

            if (armyAlreadyAlive)
            {
                return Ok("Your army is already alive.");
            }

            user.Bananas -= bananaCost;

            await context.SaveChangesAsync();

            return Ok("Army revived!");
        }

        [HttpGet]
        public async Task<IActionResult> GetUserUnits()
        {
            var user = await utilityService.GetUser();
            var userUnits = await context.UserUnits
                .Where(un => un.UserId == user.Id)
                .Select(un => new UserUnitResponse
                {
                    UnitId = un.UnitId,
                    HitPoints = un.HitPoints,
                })
                .ToListAsync();

            return Ok(userUnits);
        }
    }
}
