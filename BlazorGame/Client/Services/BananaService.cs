namespace BlazorGame.Client.Services;

public class BananaService : IBananaService
{
    /// <inheritdoc />
    public event Action? OnChange;

    /// <inheritdoc />
    public int Bananas { get; set; } = 1000;

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

    void BananasChanged() => OnChange?.Invoke();
}
