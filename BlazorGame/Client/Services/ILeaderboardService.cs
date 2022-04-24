using System.Net.Http.Json;
using BlazorGame.Shared;

namespace BlazorGame.Client.Services;

public interface ILeaderboardService
{
    IList<UserStatistic> Leaderboard { get; set; }
    Task GetLeaderboard();
}

public class LeaderboardService : ILeaderboardService
{
    private readonly HttpClient http;

    public LeaderboardService(HttpClient http)
    {
        this.http = http;
    }

    /// <inheritdoc />
    public IList<UserStatistic> Leaderboard { get; set; }

    /// <inheritdoc />
    public async Task GetLeaderboard()
    {
        Leaderboard = await http.GetFromJsonAsync<IList<UserStatistic>>("api/User/leaderboard");
    }
}
