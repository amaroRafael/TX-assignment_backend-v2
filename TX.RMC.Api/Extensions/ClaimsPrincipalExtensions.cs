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
    public static Guid GetId(this ClaimsPrincipal user)
    {
        if (user?.Identity?.IsAuthenticated ?? false)
        {
            var claimValue = user.FindFirstValue(ClaimTypes.Sid);

            if (!string.IsNullOrEmpty(claimValue))
            {
                if (Guid.TryParse(claimValue, out Guid id))
                    return id;
            }
        }

        return Guid.Empty;
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
