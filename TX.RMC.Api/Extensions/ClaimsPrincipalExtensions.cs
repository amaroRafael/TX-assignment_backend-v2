namespace TX.RMC.Api.Extensions;

using System.Security.Claims;

/// <summary>
/// Extension methods for <see cref="ClaimsPrincipal"/>.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Get the user id from the claims principal.
    /// </summary>
    public static string? GetId(this ClaimsPrincipal user)
    {
        if (user?.Identity?.IsAuthenticated ?? false)
        {
            return user.FindFirstValue(ClaimTypes.Sid);
        }

        return null;
    }

    /// <summary>
    /// Get the user name from the claims principal.
    /// </summary>
    public static string? GetNameIdentifier(this ClaimsPrincipal user)
    {
        if (user?.Identity?.IsAuthenticated ?? false)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        return null;
    }
}
