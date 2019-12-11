using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace RMealsAPI.Code.Extensions
{
    public static class IdentityResultExtensions
    {
        public static string GetErrorString(this IdentityResult identityResult)
        {
            return identityResult.Errors.Select(i => i.Description).Aggregate(string.Empty, (current, next) => current + (string.IsNullOrWhiteSpace(current) ? "" : ", ") + next);
        }
        
    }

}