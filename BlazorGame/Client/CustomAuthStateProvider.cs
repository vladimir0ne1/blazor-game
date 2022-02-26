using System.Security.Claims;
using Blazored.LocalStorage;
using BlazorGame.Shared;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorGame.Client;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private static readonly AuthenticationState UnAuthenticatedState = new(new ClaimsPrincipal());

    private readonly ILocalStorageService localStorageService;

    /// <inheritdoc />
    public CustomAuthStateProvider(ILocalStorageService localStorageService)
    {
        this.localStorageService = localStorageService;
    }

    /// <inheritdoc />
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var state = UnAuthenticatedState;
        if (await localStorageService.GetItemAsync<bool>(Constants.IsAuthenticated))
        {
            var userName = await localStorageService.GetItemAsync<string>(nameof(UserLogin.UserName));
            var nameClaim = new Claim(ClaimTypes.Name, userName);
            var identity = new ClaimsIdentity(new[] { nameClaim }, "test authentication type");

            var user = new ClaimsPrincipal(identity);
            state = new AuthenticationState(user);
        }

        NotifyAuthenticationStateChanged(Task.FromResult(state));

        return state;
    }
}
