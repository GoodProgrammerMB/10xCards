using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FlashCard.Api.Data;
using FlashCard.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BC = BCrypt.Net.BCrypt;

namespace FlashCard.Api.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterModel model);
    Task<AuthResponse> LoginAsync(LoginModel model);
    string GenerateJwtToken(User user);
}

public class AuthService : IAuthService
{
    private readonly FlashCardDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(FlashCardDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterModel model)
    {
        if (await _context.Users.AnyAsync(u => u.Email == model.Email))
        {
            return new AuthResponse
            {
                Successful = false,
                Error = "Użytkownik o podanym adresie email już istnieje."
            };
        }

        var user = new User
        {
            Email = model.Email,
            Username = model.Email,
            PasswordHash = BC.HashPassword(model.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            Successful = true,
            Token = GenerateJwtToken(user),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email
            }
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginModel model)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

        if (user == null || !BC.Verify(model.Password, user.PasswordHash))
        {
            return new AuthResponse
            {
                Successful = false,
                Error = "Nieprawidłowy email lub hasło."
            };
        }

        return new AuthResponse
        {
            Successful = true,
            Token = GenerateJwtToken(user),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email
            }
        };
    }

    public string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured");
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is not configured");
        var jwtAudience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is not configured");
        var jwtExpiryMinutes = _configuration["Jwt:ExpiryInMinutes"] ?? "60";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddMinutes(Convert.ToDouble(jwtExpiryMinutes));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
} 