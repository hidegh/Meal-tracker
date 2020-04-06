using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NetCore3.Persistence;
using System.Net;
using RMealsAPI.Code.Extensions;

namespace RMealsAPI.Code.Filters
{
    /// <summary>
    /// Originally impl. as TypeFilter due DI, but later DB call was dropped...
    /// </summary>
	public class AuthorizeSelfOrRoleAttribute : TypeFilterAttribute
    {
        public AuthorizeSelfOrRoleAttribute(string routeUserIdParam, params string[] roles) : base(typeof(InjectFilterImpl))
        {
            Arguments = new object[] { routeUserIdParam, roles };
        }

        private class InjectFilterImpl : IActionFilter
        {
            private readonly MealsDbContext dbContext;
            private readonly string routeUserIdParam;
            private readonly string[] roles;

            public InjectFilterImpl(MealsDbContext dbContext, string routeUserIdParam, params string[] roles)
            {
                this.dbContext = dbContext;
                this.routeUserIdParam = routeUserIdParam;
                this.roles = roles ?? new string[0];
            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                // Check route correesponds to current user
                var routeUserIdString = context.RouteData.Values[routeUserIdParam].ToString();
                var identityUserId = context.HttpContext.User.Identity.GetUserId();
                if (identityUserId.ToString() == routeUserIdString)
                    return;

                // Check user is in ano of the roles
                foreach (var roleName in roles)
                {
                    if (context.HttpContext.User.IsInRole(roleName))
                        return;
                }

                // Chekc failed
                context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
                
            }

        }
    }
}