using BlazorGame.Server.Data;
using BlazorGame.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorGame.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UnitController : ControllerBase
{
    private readonly DataContext context;

    public UnitController(DataContext context)
    {
        this.context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var units = await context.Units.ToListAsync();
        return Ok(units);
    }

    [HttpPost]
    public async Task<IActionResult> Post(Unit unit)
    {
        context.Units.Add(unit);
        await context.SaveChangesAsync();

        return Ok(await context.Units.ToListAsync());
    }

    [HttpPut]
    public async Task<IActionResult> Put(Unit unit)
    {

        var dbUnit = await context.Units.FirstOrDefaultAsync(u => u.Id == unit.Id);
        if (dbUnit == null)
        {
            return NotFound("Unit not found");
        }

        dbUnit.Attack = unit.Attack;
        dbUnit.Defense = unit.Defense;
        dbUnit.Name = unit.Name;
        dbUnit.BananaCost = unit.BananaCost;
        dbUnit.HitPoints = unit.HitPoints;

        await context.SaveChangesAsync();

        return Ok(await context.Units.ToListAsync());
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {

        var dbUnit = await context.Units.FirstOrDefaultAsync(u => u.Id == id);
        if (dbUnit == null)
        {
            return NotFound("Unit not found");
        }

        context.Units.Remove(dbUnit);

        await context.SaveChangesAsync();

        return Ok(await context.Units.ToListAsync());
    }
}
