using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BlazorGame.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BlazorGame.Server.Data;

public interface IAuthRepository
{
    Task<ServiceResponse<int>> Register(User user, string password, int startUnitId);

    /// <returns>Token</returns>
    Task<ServiceResponse<string>> Login(string email, string password);

    Task<bool> UserExists(string email);
}

public class AuthRepository : IAuthRepository
{
    private readonly DataContext context;
    private readonly IConfiguration configuration;

    public AuthRepository(DataContext context, IConfiguration configuration)
    {
        this.context = context;
        this.configuration = configuration;
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<int>> Register(User user, string password, int startUnitId)
    {
        if (await UserExists(user.Email))
        {
            return new ServiceResponse<int>
            {
                Data = -1, Success = false, Message = "User already exists"
            };
        }

        CreatePasswordHash(password, out var pwdHash, out var pwdSalt);
        user.Password = pwdHash;
        user.Salt = pwdSalt;

        context.Users.Add(user);
        await context.SaveChangesAsync();

        await AddStartingUnit(user, startUnitId);

        return  new ServiceResponse<int>
        {
            Data = user.Id, Success = true, Message = "Registration successfull"
        };
    }

    private async Task AddStartingUnit(User user, int startUnitId)
    {
        var unit = await context.Units.FindAsync(startUnitId);
        context.UserUnits.Add(new UserUnit
        {
            UnitId = unit.Id,
            UserId = user.Id,
            HitPoints = unit.HitPoints,
        });
        await context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<string>> Login(string email, string password)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            return new ServiceResponse<string>
            {
                Message = "User not found",
                Success = false,
            };
        }

        if (VerifyPasswordHash(password, user.Password, user.Salt))
        {
            return new ServiceResponse<string>
            {
                Data = CreateToken(user),
            };
        }

        return new ServiceResponse<string>
        {
            Message = "Wrong credentials",
            Success = false
        };
    }

    /// <inheritdoc />
    public async Task<bool> UserExists(string email)
    {
        if (email == null)
        {
            throw new ArgumentNullException(nameof(email));
        }

        return await context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }

    private bool VerifyPasswordHash(string password, byte[] pwdHash, byte[] pwdSalt)
    {
        using var hmac = new HMACSHA512(pwdSalt);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        return hash.SequenceEqual(pwdHash);
    }

    private void CreatePasswordHash(string pwd, out byte[] pwdHash, out byte[] pwdSalt)
    {
        using var hmac = new HMACSHA512();
        pwdSalt = hmac.Key;
        pwdHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(pwd));
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
        };

        var secretKey = configuration.GetSection("AppSettings:Token").Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}
