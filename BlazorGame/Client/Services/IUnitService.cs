using System.Net;
using System.Net.Http.Json;
using Blazored.Toast.Services;
using BlazorGame.Shared;

namespace BlazorGame.Client.Services;

public interface IUnitService
{
    IList<Unit> Units { get; }

    public IList<UserUnit> MyUnits { get; set; }

    Task AddUnit(int unitId);

    Task LoadUnitsAsync();

    Task LoadUserUnitsAsync();

    Task ReviveArmy();
}

class UnitService : IUnitService
{
    private readonly IToastService toastService;
    private readonly HttpClient http;
    private readonly IBananaService bananaService;

    public UnitService(IToastService toastService, HttpClient http, IBananaService bananaService)
    {
        this.toastService = toastService;
        this.http = http;
        this.bananaService = bananaService;
    }

    /// <inheritdoc />
    public IList<Unit> Units { get; private set; } = new List<Unit>();

    /// <inheritdoc />
    public IList<UserUnit> MyUnits { get; set; } = new List<UserUnit>();

    /// <inheritdoc />
    public async Task AddUnit(int unitId)
    {
        var unit = Units.First(unit => unit.Id == unitId);

        var result = await http.PostAsJsonAsync<int>("api/userunit", unitId);
        if (result.StatusCode != HttpStatusCode.OK)
        {
            toastService.ShowError(await result.Content.ReadAsStringAsync());
        }
        else
        {
            await bananaService.GetBananas();
            toastService.ShowSuccess($"Your {unit.Name} has been built!", "Unit Built!");
        }
    }

    /// <inheritdoc />
    public async Task LoadUnitsAsync()
    {
        if (Units.Count == 0)
        {
            Units = await http.GetFromJsonAsync<IList<Unit>>("api/Unit");
        }
    }

    /// <inheritdoc />
    public async Task LoadUserUnitsAsync()
    {
        MyUnits = await http.GetFromJsonAsync<IList<UserUnit>>("api/UserUnit");
    }

    public async Task ReviveArmy()
    {
        var result = await http.PostAsJsonAsync<string>("api/userunit/revive", null);
        if (result.StatusCode == HttpStatusCode.OK)
        {
            toastService.ShowSuccess(await result.Content.ReadAsStringAsync());
        }
        else
        {
            toastService.ShowError(await result.Content.ReadAsStringAsync());
        }

        await LoadUserUnitsAsync();
        await bananaService.GetBananas();
    }
}
