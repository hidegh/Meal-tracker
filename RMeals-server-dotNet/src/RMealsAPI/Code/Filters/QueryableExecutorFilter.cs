using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RMealsAPI.Code.Extensions;

/// <summary>
/// When returning deferred queries, they will be executed when formatter converts them.
/// This is outside the action context - so any errors inside the query won't be catched by IExceptionFilter.
/// 
/// This filter actually allows us to execute the query before we leave the action context.
/// Also a possible solution is to use a middleware above the MVC.
/// </summary>
namespace RMealsAPI.Code.Filters
{
   	/// <summary>
	/// When returning deferred queries, they will be executed when formatter executes while trying to convert them.
	/// This task is performed outside the action context - so any errors inside the query won't be catched by IExceptionFilter.
	/// 
	/// This filter actually allows us to execute the query before we leave the action context.
	/// 
	/// Also a possible solution is to use a middleware above the MVC middleware to catch exceptions and return error details in a pre-defined structure.
	/// </summary>
	public class QueryableExecutorFilter : IActionFilter
	{
		public void OnActionExecuting(ActionExecutingContext context)
		{

		}

		public void OnActionExecuted(ActionExecutedContext context)
		{
			if (context.Exception != null)
				return;

			var responseContent = context.Result as ObjectResult;

			if (responseContent != null)
			{
				var queryable = responseContent.Value as IQueryable;
				if (queryable != null)
				{
					// use prepared extension method on iqueryable (ienumerable)
					var result = queryable.ToListExecutor();

					// override original response
					responseContent.Value = result;
				}
			}

		}
	}
}
