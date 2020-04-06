using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetCore3.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RMealsAPI.Code.Filters;
using RMealsAPI.Model;
using RMealsAPI.Persistence;
using RMealsAPI.Code.Extensions;
using Microsoft.EntityFrameworkCore;
using System;

namespace RMealsAPI.Features.Meals
{
    /// <summary>
    /// NOTE: the omitting of the default version and such route is not out-of the box, not that route is added both to the v1 as v2 docs!
    /// </summary>
    [ApiController]
    [Route("users/{userId}/[controller]")]
    [ApiVersion("1.0")]
    [Route("v{v:apiVersion}/users/{userId}/[controller]")]
    [AuthorizeSelfOrRoleAttribute("userId", RoleConsts.Admin)]
    public class MealsController : ControllerBase
    {
        private ILogger<MealsController> logger;
        private MealsDbContext dbContext;

        public MealsController(ILogger<MealsController> logger, MealsDbContext dbContext)
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        [HttpGet]
        [Route("{mealId}")]
        public async Task<ActionResult<MealDto>> GetMeal(long userId, long mealId)
        {
            var mealQuery =
                from u in dbContext.Set<User>()
                from m in u.Meals
                where u.Id == userId && m.Id == mealId
                select new MealDto()
                {
                    Date = m.Date,
                    Calories = m.Calories,
                    Description = m.Description
                };

            var meal = mealQuery.First();
            return Ok(meal);
        }

        /// <summary>
        /// Creates a new meal entry for the given user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mealId"></param>
        /// <param name="mealDto"></param>
        /// <returns>Unique ID of the given UserMeal entry; The Location header will also be set and the URL to access the newly created item will be set there.</returns>
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateMeal(long userId, [FromBody] MealDto mealDto)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            long? mealId = null;

            using (var tx = TransactionScopeBuilder.New())
            {
                var user = dbContext
                    .Set<User>()
                    .First(u => u.Id == userId);

                // NOTE: with DDD it would be a user.AddMeal(date, calories, desc) where inside I'd set the back-ref to User as well (always valid object graph)!
                var meal = new UserMeal()
                {
                    Date = mealDto.Date,
                    Calories = mealDto.Calories,
                    Description = mealDto.Description
                };

                user.Meals.Add(meal);

                // it's not NH, so to get the ID we need to save
                await dbContext.SaveChangesAsync();

                mealId = meal.Id;

                tx.Complete();
            }

            return this.Created(mealId);
        }

        [HttpPut]
        [Route("{mealId}")]
        public async Task<IActionResult> UpdateMeal(long userId, long mealId, [FromBody] MealDto mealDto)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            using (var tx = TransactionScopeBuilder.New())
            {
                var meal = (
                    from u in dbContext.Set<User>()
                    from m in u.Meals
                    where u.Id == userId && m.Id == mealId
                    select m
                ).First();

                meal.Date = mealDto.Date;
                meal.Calories = mealDto.Calories;
                meal.Description = mealDto.Description;

                await dbContext.SaveChangesAsync();

                tx.Complete();
            }

            return Ok();
        }

        [HttpDelete]
        [Route("{mealId}")]
        public async Task<IActionResult> DeleteMeal(long userId, long mealId)
        {
            using (var tx = TransactionScopeBuilder.New())
            {
                var result = (
                    from u in dbContext.Set<User>()
                    from m in u.Meals
                    where u.Id == userId && m.Id == mealId
                    select new { User = u, Meal = m }
                ).First();

                // NOTE: remove will just unassign if the 1:N is mapped without IsRequired (and will delete if ISRequired was used at mapping)
                result.User.Meals.Remove(result.Meal);

                await dbContext.SaveChangesAsync();

                tx.Complete();
            }

            return Ok();
        }

        /// <summary>
        /// Gets a list of mails (with possible filtering).
        /// NOTE: originally would have used OData (flexible filtering and KENDO has support for creating queries in a structured fashion), but within .NET Core 3 OData is not functional yet (see: https://github.com/OData/WebApi/issues/1748).
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ODataQueryableAttribute]
        public async Task<ActionResult<IEnumerable<MealDailySummaryDto>>> GetMeals(
            long userId,
            [FromQuery] DateTime? dateFrom,
            [FromQuery] DateTime? dateTo,
            [FromQuery] TimeSpan? timeFrom,
            [FromQuery] TimeSpan? timeTo
            )
        {
            var user = dbContext.Set<User>().Include(u => u.Profile).First(u => u.Id == userId);

            var mealFilterOptionsDto = new MealFilterOptionsDto()
            {
                DateFrom = dateFrom,
                DateTo = dateTo,
                TimeFrom = timeFrom,
                TimeTo = timeTo
            };

            // NOTE:
            // separate query, fetching from DB just what we need
            // ...the join is needed as there's no meal.User association
            // ...and due DDD reasons no FKID and as EF would so-so hit DB when accessing foreign property id's (despite that ID being on the given entity-table)
            var mealsDailySummaryQuery =
                from u in dbContext.Set<User>().Include(u => u.Profile).DefaultIfEmpty()
                from m in u.Meals
                where u.Id == userId
                group m by m.Date.Date into g
                orderby g.Key descending
                select new
                {
                    Day = g.Key,
                    MealsCount = g.Count(),
                    DailyCaloriesConsumed = g.Sum(m => m.Calories),
                    DailyCaloriesExceeded = g.Sum(m => m.Calories) > (user.Profile != null ? user.Profile.AllowedCalories : 0)
                    // NOTE: must find a way to assign an empty N:1 object that will work (do a save) when altering it value (default assumption: no N:1 object in DB by def.)
                };

            /*
             * EF Core limitations:
             * 
             *  https://github.com/aspnet/EntityFrameworkCore/issues/17878
             *  https://github.com/aspnet/EntityFrameworkCore/issues/17068
             *  https://github.com/dotnet/docs/issues/15649
             * 
             *  ...this forces to do partial work outside SQL server
             *  ...also this could be something that would be possibly a blocker with OData (performance issues)
             * 
             *  Roadmap:
             *  https://docs.microsoft.com/en-us/ef/core/what-is-new/index
             *  
             * OData limitations:
             *  https://github.com/OData/WebApi/issues/1748 (with .NET Core 3)
             *  
             */

            var mealsTimeFilteredQuery =
                 from u in dbContext.Set<User>()
                 from m in u.Meals
                 where u.Id == userId
                 orderby m.Date ascending
                 select m;

            if (mealFilterOptionsDto.DateFrom != null)
            {
                mealsDailySummaryQuery = mealsDailySummaryQuery.Where(m => m.Day >= mealFilterOptionsDto.DateFrom.Value.Date);
                mealsTimeFilteredQuery = mealsTimeFilteredQuery.Where(m => m.Date >= mealFilterOptionsDto.DateFrom.Value.Date);
            }
            if (mealFilterOptionsDto.DateTo != null)
            {
                mealsDailySummaryQuery = mealsDailySummaryQuery.Where(m => m.Day < mealFilterOptionsDto.DateTo.Value.Date);
                mealsTimeFilteredQuery = mealsTimeFilteredQuery.Where(m => m.Date < mealFilterOptionsDto.DateTo.Value.Date);
            }

            if (mealFilterOptionsDto.TimeFrom != null)
                mealsTimeFilteredQuery = mealsTimeFilteredQuery.Where(m => m.Date.TimeOfDay >= mealFilterOptionsDto.TimeFrom.Value);
            if (mealFilterOptionsDto.TimeTo != null)
                mealsTimeFilteredQuery = mealsTimeFilteredQuery.Where(m => m.Date.TimeOfDay < mealFilterOptionsDto.TimeTo.Value);

            // In memory grouping due EF core limitations
            var mealsDailySummary = mealsDailySummaryQuery.ToList();
            var meals = mealsTimeFilteredQuery.ToList();

            var groupQuery =
                from ms in mealsDailySummary
                select new MealDailySummaryDto()
                {
                    Day = ms.Day,
                    MealsCount = ms.MealsCount,
                    DailyCaloriesConsumed = ms.DailyCaloriesConsumed,
                    DailyCaloriesExceeded = ms.DailyCaloriesExceeded,
                    Meals =
                        from m in meals
                        where m.Date.Date == ms.Day
                        select new MealDailySummaryDto.MealDailyItemDto()
                        {
                            Id = m.Id,
                            Date = m.Date,
                            Calories = m.Calories,
                            Description = m.Description
                        }
                };

            // NOTE: 
            // groupQuery contains entry for each day, even if there is no meal on a given time-interval
            // we could filter out those entries (but IMO it's nicer to have them there too
            // groupQuery = groupQuery.Where(i => i.DailyMeals.Count() > 0);

            var result = groupQuery; // .ToList();
            return Ok(result);
        }

        /// <summary>
        /// Gets a list of mails (with possible filtering).
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("advanced")]
        [ODataQueryableAttribute]
        public async Task<ActionResult<IEnumerable<MealDailySummaryDto>>> GetMealsAdvanced(
            long userId)
        {
            var user = dbContext.Set<User>().Include(u => u.Profile).First(u => u.Id == userId);

            var mealsDailySummaryQuery =
                from u in dbContext.Set<User>()
                from m in u.Meals
                group m by m.Date.Date into g
                select new MealDailySummaryDto()
                {
                    Day = g.Key,
                    MealsCount = g.Count(),
                    DailyCaloriesConsumed = g.Sum(m => m.Calories),
                    DailyCaloriesExceeded = g.Sum(m => m.Calories) > user.Profile.AllowedCalories,
                    Meals = g
                        .Select(m => new MealDailySummaryDto.MealDailyItemDto()
                        {
                            Id = m.Id,
                            Date = m.Date.Date,
                            Calories = m.Calories,
                            Description = m.Description
                        })
                };

            return Ok(mealsDailySummaryQuery);
        }
    }
}



/*
    //
    // NOTE:
    // the code below is for EF6 (EDMX) - some names were slightly changed (due EDMX conventions)
    // works as a one shot, single SQL call version
    //
    // special "care" on date/time format is needed

    var dbContext = new TTMealsEntities();
            
    var user = dbContext.Set<AspNetUsers>().Include(u => u.UserProfile).First(u => u.Id == userId);

    var fromTimeStr = mealFilterOptionsDto.TimeFrom != null ? mealFilterOptionsDto.TimeFrom.Value.ToString(@"hh\:mm\:ss\.fffffff") : null;
    var toTimeStr = mealFilterOptionsDto.TimeTo != null ? mealFilterOptionsDto.TimeTo.Value.ToString(@"hh\:mm\:ss\.fffffff") : null;

    var mealsDailySummaryQuery =
        from u in dbContext.Set<AspNetUsers>()
        from m in u.UserMeal
        // NOTE: no need to "trim" dates here: DbFunctions.TruncateTime(m.Date)
        where (mealFilterOptionsDto.DateFrom == null || m.Date >= mealFilterOptionsDto.DateFrom) && (mealFilterOptionsDto.DateTo == null || m.Date < mealFilterOptionsDto.DateTo)
        group m by DbFunctions.TruncateTime(m.Date) into g
        select new
        {
            Day = g.Key,
            MealsCount = g.Count(),
            DailyCaloriesConsumed = g.Sum(m => m.Calories),
            DailyCaloriesExceeded = g.Sum(m => m.Calories) > user.UserProfile.AllowedCalories,
            Meals = g
                // NOTE: just string based comparison works for time-parts, or computed column (inside VIEW !!!)
                .Where(m => (mealFilterOptionsDto.TimeFrom == null || string.Compare(m.Date.ToString().Substring(11), fromTimeStr) >= 0) && (mealFilterOptionsDto.TimeTo == null || string.Compare(m.Date.ToString().Substring(11), toTimeStr) < 0))
                .Select(m => new
                {
                    m.Id,
                    m.Date,
                    m.Description,
                    m.Calories
                })
        };

    //
    // NOTE:
    // the same with EF core won't work until grouping issues are not solved there

    var user = dbContext.Set<AspNetUsers>().Include(u => u.Profile).First(u => u.Id == userId);

    var mealsDailySummaryQuery =
        from u in dbContext.Set<User>()
        from m in u.Meals
        where (mealFilterOptionsDto.DateFrom == null || m.Date >= mealFilterOptionsDto.DateFrom) && (mealFilterOptionsDto.DateTo == null || m.Date < mealFilterOptionsDto.DateTo)
        group m by m.Date.Date into g
        select new
        {
            Day = g.Key,
            MealsCount = g.Count(),
            DailyCaloriesConsumed = g.Sum(m => m.Calories),
            DailyCaloriesExceeded = g.Sum(m => m.Calories) > user.Profile.AllowedCalories,
            Meals = g
                .Where(m => (mealFilterOptionsDto.TimeFrom == null || m.Date.TimeOfDay >= mealFilterOptionsDto.TimeFrom) && (mealFilterOptionsDto.TimeTo == null || m.Date.TimeOfDay < mealFilterOptionsDto.TimeTo))
                .Select(m => new
                {
                    m.Id,
                    m.Date,
                    m.Description,
                    m.Calories
                })
        };

 */
