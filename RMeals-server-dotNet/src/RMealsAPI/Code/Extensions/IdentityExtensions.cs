using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace RMealsAPI.Code.Extensions
{
    public static class IdentityExtensions
    {
        public static long GetUserId(this IIdentity identity)
        {
            var userIdString = ((ClaimsIdentity)identity)
                .Claims
                .First(i => i.Type == ClaimTypes.NameIdentifier)
                .Value;

            return long.Parse(userIdString);
        }

        public static string[] GetRoles(this IIdentity identity)
        {
            return ((ClaimsIdentity)identity)
                .Claims
                .Where(i => i.Type == ClaimTypes.Role)
                .Select(i => i.Value)
                .ToArray();
        }

    }

}