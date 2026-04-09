namespace Thakkirni.API.Application.Common.Models;

public sealed class CurrentUserContext
{
    public int UserId { get; init; }
    public string Role { get; init; } = "USER";
}
