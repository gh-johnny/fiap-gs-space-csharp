namespace OrbitalGuardian.Application.Interfaces;

/// <summary>Provides password hashing and verification operations.</summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
