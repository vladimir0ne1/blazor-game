using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using BlazorGame.Client.Services;
using BlazorGame.Shared;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorGame.Client;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService localStorageService;
    private readonly HttpClient http;
    private readonly IBananaService bananaService;

    /// <inheritdoc />
    public CustomAuthStateProvider(
        ILocalStorageService localStorageService,
        HttpClient http,
        IBananaService bananaService)
    {
        this.localStorageService = localStorageService;
        this.http = http;
        this.bananaService = bananaService;
    }

    /// <inheritdoc />
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        http.DefaultRequestHeaders.Authorization = null; // reset auth header in case of unauthorized;

        var authToken = await localStorageService.GetItemAsync<string>(Constants.AuthToken);

        var identity = new ClaimsIdentity();
        if (!string.IsNullOrEmpty(authToken))
        {
            try
            {
                identity = new ClaimsIdentity(ParseClaimsFromJwt(authToken), "jwt");
                http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", authToken.Replace("\"", ""));
                await bananaService.GetBananas();
            }
            catch
            {
                await localStorageService.RemoveItemAsync(Constants.AuthToken);
                identity = new ClaimsIdentity();
            }
        }

        var user = new ClaimsPrincipal(identity);
        var state = new AuthenticationState(user);

        NotifyAuthenticationStateChanged(Task.FromResult(state));

        return state;
    }

    private byte[] ParseBase64WithoutPadding(string base64)
    {
        var paddedBase64 = base64;
        switch (paddedBase64.Length % 4)
        {
            case 2: paddedBase64 += "==";
                break;
            case 3: paddedBase64 += "=";
                break;
        }

        return Convert.FromBase64String(paddedBase64);
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var claims = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        return claims.Select(kv => new Claim(kv.Key, kv.Value.ToString()));
    }
}
