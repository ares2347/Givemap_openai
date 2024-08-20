using Microsoft.AspNetCore.Identity;

namespace GiveMap_Backend.Models;

public class User : IdentityUser
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public string Role { get; set; }
    public string PasswordResetToken { get; set; } = string.Empty;
    public DateTime? PasswordResetTokenExpires { get; set; }
    public DateTime LastLoginDate { get; set; }
}