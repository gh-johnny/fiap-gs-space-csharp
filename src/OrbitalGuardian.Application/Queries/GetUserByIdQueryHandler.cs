using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Exceptions;

namespace OrbitalGuardian.Application.Queries;

public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserResponse>
{
    private readonly IUserRepository _repo;

    public GetUserByIdQueryHandler(IUserRepository repo) => _repo = repo;

    public async Task<UserResponse> HandleAsync(GetUserByIdQuery query, CancellationToken ct)
    {
        var user = await _repo.GetByIdAsync(query.Id, ct)
            ?? throw new UserNotFoundException(query.Id.ToString());
        return new UserResponse
        {
            Id = user.Id, Email = user.Email, FullName = user.FullName,
            Role = user.Role, CreatedAt = user.CreatedAt, IsActive = user.IsActive
        };
    }
}
