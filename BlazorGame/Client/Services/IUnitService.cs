using Blazored.Toast.Services;
using BlazorGame.Shared;

namespace BlazorGame.Client.Services;

public interface IUnitService
{
    IList<Unit> Units { get; }

    public IList<UserUnit> MyUnits { get; set; }

    void AddUnit(int unitId);
}

class UnitService : IUnitService
{
    private readonly IToastService toastService;

    public UnitService(IToastService toastService)
    {
        this.toastService = toastService;
    }

    /// <inheritdoc />
    public IList<Unit> Units { get; } = new List<Unit>
    {
        new() { Id = 1, Name = "Knight", Attack = 10, Defense = 10, BananaCost = 100 },
        new() { Id = 2, Name = "Archer", Attack = 15, Defense = 5, BananaCost = 150 },
        new() { Id = 3, Name = "Mage", Attack = 20, Defense = 1, BananaCost = 200 },
    };

    /// <inheritdoc />
    public IList<UserUnit> MyUnits { get; set; } = new List<UserUnit>();

    /// <inheritdoc />
    public void AddUnit(int unitId)
    {
        var unit = Units.First(unit => unit.Id == unitId);
        MyUnits.Add(new UserUnit { UnitId = unit.Id, HitPoints = unit.HitPoints });
        toastService.ShowSuccess($"Your {unit.Name} has been built!", "Unit Built!");
    }
}
