using System.Net.Http.Json;

namespace BlazorGame.Client.Services;

public interface IBananaService
{
    event Action OnChange;

    int Bananas { get; set; }

    void EatBananas(int amount);

    Task AddBananas(int amount);

    Task GetBananas();
}

public class BananaService : IBananaService
{
    private readonly HttpClient http;

    public BananaService(HttpClient http)
    {
        this.http = http;
    }

    /// <inheritdoc />
    public event Action OnChange;

    /// <inheritdoc />
    public int Bananas { get; set; }

    /// <inheritdoc />
    public void EatBananas(int amount)
    {
        Bananas -= amount;
        BananasChanged();
    }

    public async Task AddBananas(int amount)
    {
        var result = await http.PutAsJsonAsync("api/User/addbananas", amount);
        Bananas = await result.Content.ReadFromJsonAsync<int>();
        BananasChanged();
    }

    /// <inheritdoc />
    public async Task GetBananas()
    {
        Bananas = await http.GetFromJsonAsync<int>("api/User/getbananas");
        BananasChanged();
    }

    void BananasChanged() => OnChange?.Invoke();
}
