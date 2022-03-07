using System.Net.Http.Json;
using Blazored.Toast.Services;
using BlazorGame.Shared;

namespace BlazorGame.Client.Services;

public interface IUnitService
{
    IList<Unit> Units { get; }

    public IList<UserUnit> MyUnits { get; set; }

    void AddUnit(int unitId);

    Task LoadUnitsAsync();
}

class UnitService : IUnitService
{
    private readonly IToastService toastService;
    private readonly HttpClient http;

    public UnitService(IToastService toastService, HttpClient http)
    {
        this.toastService = toastService;
        this.http = http;
    }

    /// <inheritdoc />
    public IList<Unit> Units { get; private set; } = new List<Unit>();

    /// <inheritdoc />
    public IList<UserUnit> MyUnits { get; set; } = new List<UserUnit>();

    /// <inheritdoc />
    public void AddUnit(int unitId)
    {
        var unit = Units.First(unit => unit.Id == unitId);
        MyUnits.Add(new UserUnit { UnitId = unit.Id, HitPoints = unit.HitPoints });
        toastService.ShowSuccess($"Your {unit.Name} has been built!", "Unit Built!");
    }

    /// <inheritdoc />
    public async Task LoadUnitsAsync()
    {
        if (Units.Count == 0)
        {
            Units = await http.GetFromJsonAsync<IList<Unit>>("api/Unit");
        }
    }
}
