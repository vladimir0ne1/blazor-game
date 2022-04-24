using System.Security.Claims;
using BlazorGame.Server.Data;
using BlazorGame.Shared;
namespace BlazorGame.Server.Services;

public interface IUtilityService
{
    Task<User> GetUser();
}

public class UtilityService : IUtilityService
{
    private readonly DataContext context;
    private readonly IHttpContextAccessor httpContextAccessor;

    public UtilityService(DataContext context, IHttpContextAccessor httpContextAccessor)
    {
        this.context = context;
        this.httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public async Task<User> GetUser()
    {
        var userId = int.Parse(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        return await context.Users.FindAsync(userId);
    }
}
