using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TradeIt.API.Data;
using TradeIt.API.DTOs;
using TradeIt.API.Models;

namespace TradeIt.API.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(
        ApplicationDbContext context,
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<object> RegisterAsync(RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        var existingUser = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email);

        if (existingUser != null)
            throw new Exception("Email already registered.");

        var user = new User
        {
            Name = dto.Name,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Phone = dto.Phone,
            Location = dto.Location
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new
        {
            message = "Registration successful.",
            user.Id,
            user.Name,
            user.Email
        };
    }

    public async Task<object?> LoginAsync(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email);

        if (user == null)
            return null;

        var validPassword =
            BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

        if (!validPassword)
            return null;

        var token = GenerateToken(user);

        return new
        {
            token,
            user = new
            {
                user.Id,
                user.Name,
                user.Email,
                user.Phone,
                user.Location
            }
        };
    }

    private string GenerateToken(User user)
    {
        var key = _configuration["Jwt:Key"]
                  ?? throw new Exception("JWT key missing.");

        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var securityKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

        var credentials =
            new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(
                    _configuration["Jwt:DurationInMinutes"] ?? "60")),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}