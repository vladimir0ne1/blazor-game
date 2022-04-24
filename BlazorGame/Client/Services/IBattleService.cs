using System.Net.Http.Json;
using BlazorGame.Shared;

public interface IBattleService
{
    Task<BattleResult> StartBattle(int opponentId);
    Task GetHistory();

    IList<BattleHistory> History { get; set; }
    BattleResult LastBattle { get; set; }
}

public class BattleService : IBattleService
{
    private readonly HttpClient http;

    public BattleService(HttpClient http)
    {
        this.http = http;
    }

    public BattleResult LastBattle { get; set; } = new BattleResult();

    /// <inheritdoc />
    public async Task<BattleResult> StartBattle(int opponentId)
    {
        var result = await http.PostAsJsonAsync("api/battle", opponentId);
        LastBattle = await result.Content.ReadFromJsonAsync<BattleResult>();
        return LastBattle;
    }

    /// <inheritdoc />
    public async Task GetHistory()
    {
        History = await http.GetFromJsonAsync<IList<BattleHistory>>("api/User/history");
    }

    /// <inheritdoc />
    public IList<BattleHistory> History { get; set; } = new List<BattleHistory>();
}
