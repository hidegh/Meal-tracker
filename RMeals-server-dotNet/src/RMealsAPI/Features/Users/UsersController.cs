using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetCore3.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using RMealsAPI.Code.Extensions;
using RMealsAPI.Code.Filters;
using RMealsAPI.Features.Users;
using RMealsAPI.Model;
using RMealsAPI.Persistence;

namespace RMealsAPI.Features.Meals
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private ILogger<UsersController> logger;
        private MealsDbContext dbContext;
        private UserManager<User> userManager;

        public UsersController(
            ILogger<UsersController> logger, 
            MealsDbContext dbContext,
            UserManager<User> userManager
            )
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        /// <summary>
        /// Get profile of given user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{userId}/profile")]
        [AuthorizeSelfOrRoleAttribute("userId", RoleConsts.Admin, RoleConsts.Manager)]
        public async Task<ActionResult<UserProfileDto>> GetProfile(int userId)
        {
            // NOTE: simple call, no need for separate query object
            var user = dbContext
                .Set<User>()
                .Include(u => u.Profile)
                .First(u => u.Id == userId);

            var userDto = new UserProfileDto()
            {
                AllowedCalories = user.Profile?.AllowedCalories ?? 0
            };

            return Ok(userDto);
        }

        /// <summary>
        /// Update profile of given user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userProfileDto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{userId}/profile")]
        [AuthorizeSelfOrRoleAttribute("userId", RoleConsts.Admin, RoleConsts.Manager)]
        public async Task<IActionResult> UpdateProfile(long userId, [FromBody] UserProfileDto userProfileDto)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            using (var tx = TransactionScopeBuilder.New())
            {
                // NOTE: no need for transaction *** although due the def. TX level the DbContext has (serializabe) *** we should add to have repeatable read or read committed
                var user = dbContext
                    .Set<User>()
                    .Include(u => u.Profile)
                    .First(u => u.Id == userId);

                user.Profile = user.Profile ?? new UserProfile();
                user.Profile.AllowedCalories = userProfileDto.AllowedCalories;
                
                await dbContext.SaveChangesAsync();

                tx.Complete();
            }

            return Ok();
        }

        /// <summary>
        /// List all users (or just a single one)
        /// </summary>
        /// <param name="userId">If this value is passed as a query-string, single user detail will be returned...</param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [Authorize(Roles = RoleConsts.Admin + ", " + RoleConsts.Manager)]
        public async Task<ActionResult<IEnumerable<UserDetailsDto>>> GetUsers([FromQuery] long? userId)
        {
            var userQuery = dbContext
                .Set<User>()
                .Include(u => u.Profile)
                .Select(u => new UserDetailsDto()
                {
                    Id = u.Id,
                    Name = u.UserName,
                    AllowedCalories = u.Profile != null ? u.Profile.AllowedCalories : 0
                });

            if (userId.HasValue)
                userQuery = userQuery.Where(i => i.Id == userId);

            return Ok(userQuery);
        }

        /// <summary>
        /// Get's a list of user roles...
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{userId}/roles")]
        [Authorize(Roles = RoleConsts.Admin + ", " + RoleConsts.Manager)]
        public async Task<ActionResult<string[]>> GetRolesForUser(long userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            var userRoles = await userManager.GetRolesAsync(user);
            return Ok(userRoles);
        }

        /// <summary>
        /// Set's new roles for a given user/
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newRoles"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{userId}/roles")]
        [Authorize(Roles = RoleConsts.Admin + ", " + RoleConsts.Manager)]
        public async Task<ActionResult> SetRolesForUser(long userId, [FromBody] string[] newRoles)
        {
            newRoles = newRoles ?? new string[0];

            var user = await userManager.FindByIdAsync(userId.ToString());

            // Check if we can manipulate given roles (also split to add/remove role groups)
            var myRoles = User.Identity.GetRoles();
            var myRoleWeight = myRoles.Select(r => Array.IndexOf(RoleConsts.RoleOrder, r)).DefaultIfEmpty(-1).Max();

            var oldRoles = await userManager.GetRolesAsync(user);           
            var rolesToAdd = newRoles.Where(nr => !oldRoles.Contains(nr)).ToList();
            var rolesToRemove = oldRoles.Where(or => !newRoles.Contains(or)).ToList();

            var cantManageRoles =
                rolesToAdd.Where(r => Array.IndexOf(RoleConsts.RoleOrder, r) > myRoleWeight).Union(rolesToRemove.Where(r => Array.IndexOf(RoleConsts.RoleOrder, r) > myRoleWeight)).ToList();

            if (cantManageRoles.Count > 0)
                return StatusCode((int) HttpStatusCode.Forbidden, $"You are not allowed to alter roles: {string.Join(", ", cantManageRoles)}");

            // Alter roles
            await userManager.AddToRolesAsync(user, rolesToAdd);
            await userManager.RemoveFromRolesAsync(user, rolesToRemove);

            return Ok();
        }

    }
}
