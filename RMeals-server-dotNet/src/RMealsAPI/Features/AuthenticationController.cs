using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using RMealsAPI.Code.Extensions;
using RMealsAPI.Model;

namespace RMealsAPI.Features
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private ILogger<AuthenticationController> logger;
        private IConfiguration configuration;
        private UserManager<User> userManager;
        private IPasswordHasher<User> passwordHasher;

        public AuthenticationController(ILogger<AuthenticationController> logger, IConfiguration configuration, UserManager<User> userManager, IPasswordHasher<User> passwordHasher)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.userManager = userManager;
            this.passwordHasher = passwordHasher;
        }

        /// <summary>
        /// NOTE: FromForm parameters not supported with OAPI 3.0 - https://github.com/RicoSuter/NSwag/issues/2491
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromForm] string userName, [FromForm] string password)
        {
            try
            {
                var user = await userManager.FindByNameAsync(userName);
                if (user != null)
                {
                    if (passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) == PasswordVerificationResult.Success)
                    {
                        var userClaims = await userManager.GetClaimsAsync(user);

                        // append roles as special claims
                        var userRoles = await userManager.GetRolesAsync(user);
                        userRoles.ToList().ForEach(r => userClaims.Add(new Claim(ClaimTypes.Role, r)));

                        var claims = new[] {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            // TODO: append other claims...
                            new Claim(ClaimTypes.Name, user.UserName)
                        }
                        .Union(userClaims);

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                            issuer: configuration["Jwt:Issuer"],
                            audience: configuration["Jwt:Audience"],
                            claims: claims,
                            expires: DateTime.UtcNow.AddMinutes(int.Parse(configuration["Jwt:ExpirationMinutes"])),
                            signingCredentials: creds
                            );

                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Exception thrown at logging in: {ex}");
            }

            return BadRequest("Failed to generate token");
        }

        /// <summary>
        /// NOTE: FromForm parameters not supported with OAPI 3.0 - https://github.com/RicoSuter/NSwag/issues/2491
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromForm] string userName, [FromForm] string password)
        {
            try
            {
                var user = new User() { UserName = userName };
                var userCreationResult = await userManager.CreateAsync(user, password);
                if (userCreationResult != IdentityResult.Success)
                    return BadRequest($"Could not create user: {userCreationResult.GetErrorString()}");

                return await Login(userName, password);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Exception thrown at registering: {ex}");
            }

            return BadRequest("Failed to register user");
        }

    }
}
