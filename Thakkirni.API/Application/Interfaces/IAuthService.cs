using Thakkirni.API.DTOs;

namespace Thakkirni.API.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default);
}
