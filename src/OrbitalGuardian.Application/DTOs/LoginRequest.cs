using System.ComponentModel.DataAnnotations;

namespace OrbitalGuardian.Application.DTOs;

public class LoginRequest
{
    [Required] public string Email { get; set; } = null!;
    [Required] public string Password { get; set; } = null!;
}
