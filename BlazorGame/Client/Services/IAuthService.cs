using System.Net.Http.Json;
using BlazorGame.Shared;

namespace BlazorGame.Client.Services;

public interface IAuthService
{
    Task<ServiceResponse<int>> Register(UserRegister request);
    Task<ServiceResponse<string>> Login(UserLogin request);
}

public class AuthService : IAuthService
{
    private readonly HttpClient http;

    public AuthService(HttpClient http)
    {
        this.http = http;
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<int>> Register(UserRegister request)
    {
        var res = await http.PostAsJsonAsync("api/auth/register", request);

        return await res.Content.ReadFromJsonAsync<ServiceResponse<int>>();
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<string>> Login(UserLogin request)
    {
        var res = await http.PostAsJsonAsync("api/auth/login", request);

        return await res.Content.ReadFromJsonAsync<ServiceResponse<string>>();
    }
}
