using System.Net.Http.Json;

namespace BlazorGame.Client.Services;

public class BananaService : IBananaService
{
    private readonly HttpClient http;

    public BananaService(HttpClient http)
    {
        this.http = http;
    }

    /// <inheritdoc />
    public event Action? OnChange;

    /// <inheritdoc />
    public int Bananas { get; set; }

    /// <inheritdoc />
    public void EatBananas(int amount)
    {
        Bananas -= amount;
        BananasChanged();
    }

    public void AddBananas(int amount)
    {
        Bananas += amount;
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
