using System.Security.Claims;

namespace ECommerce.Api.Extensions;

public static class ClaimsExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? user.FindFirstValue("nameid")
                    ?? user.FindFirstValue("sub");
        return int.Parse(value!);
    }
}