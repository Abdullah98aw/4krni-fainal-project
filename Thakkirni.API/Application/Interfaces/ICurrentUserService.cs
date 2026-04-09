using System.Security.Claims;
using Thakkirni.API.Application.Common.Models;

namespace Thakkirni.API.Application.Interfaces;

public interface ICurrentUserService
{
    CurrentUserContext GetRequiredContext(ClaimsPrincipal principal);
}
