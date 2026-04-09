using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Thakkirni.API.Application.Common.Exceptions;
using Thakkirni.API.Application.Common.Models;
using Thakkirni.API.Application.Interfaces;

namespace Thakkirni.API.Infrastructure.Identity;

public sealed class CurrentUserService : ICurrentUserService
{
    public CurrentUserContext GetRequiredContext(ClaimsPrincipal principal)
    {
        var idClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(idClaim, out var userId))
            throw new ApiException(StatusCodes.Status401Unauthorized, "بيانات المستخدم غير مكتملة");

        return new CurrentUserContext
        {
            UserId = userId,
            Role = principal.FindFirstValue(ClaimTypes.Role) ?? "USER"
        };
    }
}
