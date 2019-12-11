using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace RMealsAPI.Code.Identity
{
    public class EnchancedPasswordValidator<TUser> : IPasswordValidator<TUser> where TUser : class
    {
        private const string Code = "UnsecurePassword";

        public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
        {
            var username = await manager.GetUserNameAsync(user);

            if (username == password) {
                return IdentityResult.Failed(new IdentityError { Code = Code, Description = "Password must be different from login" });
            }

            if (password.Contains("password"))
            {
                return IdentityResult.Failed(new IdentityError { Code = Code, Description = "Password can't contain 'password'" });
            }

            return IdentityResult.Success;
        }
    }
}
