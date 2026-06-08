using OrbitalGuardian.Domain.Enums;

namespace OrbitalGuardian.Application.DTOs;

public class AuthTokenResponse
{
    public string Token { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public UserRole Role { get; set; }
    public DateTime ExpiresAt { get; set; }
}
