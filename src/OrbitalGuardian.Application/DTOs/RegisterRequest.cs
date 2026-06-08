using System.ComponentModel.DataAnnotations;
using OrbitalGuardian.Domain.Enums;

namespace OrbitalGuardian.Application.DTOs;

public class RegisterRequest
{
    [Required] public string Email { get; set; } = null!;
    [Required] public string Password { get; set; } = null!;
    [Required] public string FullName { get; set; } = null!;
    [Required] public UserRole Role { get; set; }
}
