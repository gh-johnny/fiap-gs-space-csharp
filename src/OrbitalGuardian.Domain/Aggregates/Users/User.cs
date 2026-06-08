using OrbitalGuardian.Domain.Enums;
using OrbitalGuardian.Domain.Exceptions;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Domain.Aggregates.Users;

public class User : AggregateRoot<Guid>
{
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string FullName { get; private set; } = null!;
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsActive { get; private set; }

    private User() { }

    private User(Guid id, string email, string passwordHash, string fullName, UserRole role) : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
        FullName = fullName;
        Role = role;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    /// <summary>Creates a new User. Email must be non-empty and contain '@'.</summary>
    public static User Create(string email, string passwordHash, string fullName, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new DuplicateEmailException(email);

        return new User(Guid.NewGuid(), email, passwordHash, fullName, role);
    }

    public void UpdateProfile(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be empty.");
        FullName = fullName;
    }

    public void ChangeRole(UserRole newRole) => Role = newRole;

    public void Deactivate() => IsActive = false;
}
