using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GiveMap_Backend.Data;
using GiveMap_Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GiveMap_Backend.Services;

public interface IUserService
{
    Task<User> RegisterUserAsync(string email, string password);
    Task<string> AuthenticateAsync(string email, string password);
    Task<bool> RequestPasswordResetAsync(string email);
    Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
    Task UpdateLastLoginDateAsync(int userId);
}

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public UserService(ApplicationDbContext context, ILogger<UserService> logger, IConfiguration configuration,
        IEmailService emailService)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
        _emailService = emailService;
    }

    public async Task<User> RegisterUserAsync(string email, string password)
    {
        try
        {
            var user = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                CreatedAt = DateTime.UtcNow,
                IsEmailConfirmed = false,
                Role = "User" // Default role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"User registered successfully: {email}");
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error registering user: {email}");
            throw;
        }
    }

    public async Task<string> AuthenticateAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            _logger.LogWarning($"Failed login attempt for user: {email}");
            return null;
        }

        var token = GenerateJwtToken(user);
        await UpdateLastLoginDateAsync(user.Id);
        _logger.LogInformation($"User authenticated: {email}");
        return token;
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<bool> RequestPasswordResetAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            _logger.LogWarning($"Password reset requested for non-existent user: {email}");
            return false;
        }

        var token = GeneratePasswordResetToken();
        user.PasswordResetToken = token;
        user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1);

        await _context.SaveChangesAsync();

        await _emailService.SendPasswordResetEmailAsync(email, token);

        _logger.LogInformation($"Password reset token generated for user: {email}");
        return true;
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.PasswordResetToken == token);
        if (user == null || user.PasswordResetTokenExpires < DateTime.UtcNow)
        {
            _logger.LogWarning($"Invalid password reset attempt for user: {email}");
            return false;
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpires = null;

        await _context.SaveChangesAsync();

        _logger.LogInformation($"Password reset successful for user: {email}");
        return true;
    }

    public async Task UpdateLastLoginDateAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.LastLoginDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Updated last login date for user ID: {userId}");
        }
    }

    private string GeneratePasswordResetToken()
    {
        return Guid.NewGuid().ToString("N");
    }
}