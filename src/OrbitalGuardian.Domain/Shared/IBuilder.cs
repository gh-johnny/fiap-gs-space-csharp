namespace OrbitalGuardian.Domain.Shared;

public interface IBuilder<T> where T : IBuildable
{
    /// <summary>Validates all fields and constructs the target object.</summary>
    T Build();
}
