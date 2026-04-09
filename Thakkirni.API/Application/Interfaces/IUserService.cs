using Thakkirni.API.DTOs;

namespace Thakkirni.API.Application.Interfaces;

public interface IUserService
{
    Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserDto>> GetScopedUsersAsync(int currentUserId, string currentRole, CancellationToken cancellationToken = default);
    Task<UserDto> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UserDto> CreateUserAsync(CreateUpdateUserDto dto, CancellationToken cancellationToken = default);
    Task<UserDto> UpdateUserAsync(int id, CreateUpdateUserDto dto, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(int id, int currentUserId, CancellationToken cancellationToken = default);
}
