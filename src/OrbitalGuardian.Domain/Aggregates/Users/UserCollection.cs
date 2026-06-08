using System.Collections;
using OrbitalGuardian.Domain.Enums;

namespace OrbitalGuardian.Domain.Aggregates.Users;

public sealed class UserCollection : IEnumerable<User>
{
    private readonly IReadOnlyList<User> _items;

    public UserCollection(IEnumerable<User> items) =>
        _items = items.ToList().AsReadOnly();

    public UserCollection FilterByRole(UserRole role) =>
        new(_items.Where(u => u.Role == role));

    public User? FindByEmail(string email) =>
        _items.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

    public IEnumerator<User> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
