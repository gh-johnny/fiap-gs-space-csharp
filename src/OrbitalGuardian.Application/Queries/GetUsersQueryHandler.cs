using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;

namespace OrbitalGuardian.Application.Queries;

public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, IReadOnlyList<UserResponse>>
{
    private readonly IUserRepository _repo;

    public GetUsersQueryHandler(IUserRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<UserResponse>> HandleAsync(GetUsersQuery query, CancellationToken ct)
    {
        var users = await _repo.GetAllAsync(ct);
        return users.Select(u => new UserResponse
        {
            Id = u.Id, Email = u.Email, FullName = u.FullName,
            Role = u.Role, CreatedAt = u.CreatedAt, IsActive = u.IsActive
        }).ToList();
    }
}
