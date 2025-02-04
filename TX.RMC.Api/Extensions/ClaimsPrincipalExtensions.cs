namespace TX.RMC.Api.Extensions;

using System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetId(this ClaimsPrincipal user)
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

        return null;
    }

    public static string? GetNameIdentifier(this ClaimsPrincipal user)
    {
        if (user?.Identity?.IsAuthenticated ?? false)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        return null;
    }
}
