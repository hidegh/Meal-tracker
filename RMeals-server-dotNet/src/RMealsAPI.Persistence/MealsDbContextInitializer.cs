using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using RMealsAPI.Model;

namespace NetCore3.Persistence
{
    /// <summary>
    /// Another option is:
    ///     https://alexcodetuts.com/2019/05/22/how-to-seed-users-and-roles-in-asp-net-core/
    /// 
    /// See issues with Startup:
    ///     https://github.com/aspnet/Identity/issues/1458
    ///     https://entityframeworkcore.com/knowledge-base/38704025/cannot-access-a-disposed-object-in-asp-net-core-when-injecting-dbcontext
    ///     
    /// Similar issue with middleware:
    ///     https://stackoverflow.com/questions/39369054/usermanager-dbcontext-already-disposed-in-middleware
    /// </summary>
    public class MealsDbContextInitializer
    {
        private readonly IHostEnvironment hostEnvironment;
        private readonly MealsDbContext context;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole<long>> roleManager;

        public MealsDbContextInitializer(
            IHostEnvironment hostEnvironment,
            MealsDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole<long>> roleManager
            )
        {
            this.hostEnvironment = hostEnvironment;
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task EnsureRole(string roleName)
        {
            var exists = await roleManager.RoleExistsAsync(roleName);
            if (!exists)
            {
                var roleCreationResult = await roleManager.CreateAsync(new IdentityRole<long>(roleName));
                if (roleCreationResult != IdentityResult.Success)
                    throw new System.Exception("Could not create role");
            }
        }

        public async Task EnsureUserWithRoles(string userName, string password, params string[] roles)
        {
            // ensure user
            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
            {
                user = new User() { UserName = userName };
                var userCreationResult = await userManager.CreateAsync(user, password);
                if (userCreationResult != IdentityResult.Success)
                    throw new System.Exception($"Could not create user, result: {userCreationResult}");

                // NOTE: we're using ID's so we need to fetch it
                user = await userManager.FindByNameAsync(userName);
            }

            // ensure roles
            var userRoleNames = await userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                // NOTE:
                // comparing non-normalized names - better option: check if has given role, add if not (even if it hits DB mutliple times)
                if (userRoleNames.Contains(role))
                    continue;

                var roleAddResult = await userManager.AddToRoleAsync(user, role);
                if (roleAddResult != IdentityResult.Success)
                    throw new System.Exception($"Could not add role to user: {roleAddResult}");
            }
        }

        dynamic breakfasts = new[] {
            new { Time = TimeSpan.Parse("4:30"), Desc = "Early breakfast", Calories = 375, Diff = 75 },
            new { Time = TimeSpan.Parse("6:30"), Desc = "Breakfast", Calories = 350, Diff = 75},
            new { Time = TimeSpan.Parse("7:45"), Desc = "Late breakfast", Calories = 300, Diff = 75},
        };

        dynamic forenoonSnacks = new[] {
            new { Time = TimeSpan.Parse("9:30"), Desc = "Forenoon snack", Calories = 250, Diff = 100 },
            new { Time = TimeSpan.Parse("10:00"), Desc = "Forenoon snack", Calories = 200, Diff = 75 },
            new { Time = TimeSpan.Parse("10:30"), Desc = "Forenoon snack", Calories = 150, Diff = 50},
        };

        dynamic lunch = new[] {
            new { Time = TimeSpan.Parse("11:00"), Desc = "Early lunch", Calories = 500, Diff = 100 },
            new { Time = TimeSpan.Parse("11:30"), Desc = "Early lunch", Calories = 500, Diff = 100 },
            new { Time = TimeSpan.Parse("12:00"), Desc = "Lunch", Calories = 500, Diff = 100 },
            new { Time = TimeSpan.Parse("12:30"), Desc = "Lunch", Calories = 500, Diff = 100},
            new { Time = TimeSpan.Parse("13:00"), Desc = "Late lunch", Calories = 500, Diff = 100},
            new { Time = TimeSpan.Parse("13:30"), Desc = "Late lunch", Calories = 500, Diff = 100},
            new { Time = TimeSpan.Parse("14:00"), Desc = "Late lunch", Calories = 500, Diff = 100},
        };

        dynamic afternoonSnacks = new[] {
            new { Time = TimeSpan.Parse("14:30"), Desc = "Afternoon snack", Calories = 250, Diff = 100 },
            new { Time = TimeSpan.Parse("15:00"), Desc = "Afternoon snack", Calories = 200, Diff = 75 },
            new { Time = TimeSpan.Parse("15:30"), Desc = "Afternoon snack", Calories = 150, Diff = 50},
        };

        dynamic dinner = new[] {
            new { Time = TimeSpan.Parse("16:30"), Desc = "Early dinner", Calories = 500, Diff = 100 },
            new { Time = TimeSpan.Parse("17:00"), Desc = "Dinner", Calories = 500, Diff = 100 },
            new { Time = TimeSpan.Parse("17:30"), Desc = "Dinner", Calories = 500, Diff = 100},
            new { Time = TimeSpan.Parse("18:00"), Desc = "Dinner", Calories = 500, Diff = 100},
            new { Time = TimeSpan.Parse("18:30"), Desc = "Late dinner", Calories = 500, Diff = 100},
        };

        dynamic secondDinner = new[] {
            new { Time = TimeSpan.Parse("20:00"), Desc = "Early 2nd dinner", Calories = 200, Diff = 50 },
            new { Time = TimeSpan.Parse("20:30"), Desc = "2nd dinner", Calories = 150, Diff = 50 },
            new { Time = TimeSpan.Parse("21:00"), Desc = "Late 2nd dinner", Calories = 100, Diff = 50 },
        };

        public int GetRandomInt()
        {
            var provider = new RNGCryptoServiceProvider();
            var byteArray = new byte[4];
            provider.GetBytes(byteArray);
            var randomInteger = BitConverter.ToInt32(byteArray, 0);
            return randomInteger;
        }

        public int GetRandomInt(int maxNumber) => GetRandomInt() % maxNumber;

        public bool GetRandomBool(double probability = 0.5) {
            var number = Math.Abs(GetRandomInt());
            return number < int.MaxValue * probability;
        }
         
        private void RandomizeMeal(User user, double probability, DateTime date, dynamic mealObject)
        {
            var addMeal = GetRandomBool(probability);
         
            if (addMeal)
            {
                var randomIndex = Math.Abs(GetRandomInt(mealObject.Length));
                var selectedMeal = mealObject[randomIndex];

                var randomizedCaloriesDiff = GetRandomInt(selectedMeal.Diff);
                var calories = selectedMeal.Calories + randomizedCaloriesDiff;

                var time = selectedMeal.Time;
                var dateTime = date.Add(time);

                user.Meals.Add(
                    new UserMeal()
                    {
                        User = user, // I set it just to keep OO graph vaid all the time

                        Date = dateTime,
                        Description = selectedMeal.Desc,
                        Calories = calories
                    }
                );
            }
        }

        public async Task EnsureDataForUser(string userName, int allowedCalories = 1800, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            dateFrom = (dateFrom != null ? dateFrom.Value : DateTime.Now.AddDays(-30)).Date;
            dateTo = (dateTo != null ? dateTo.Value : DateTime.Now).Date;

            var user = context.Set<User>().First(u => u.UserName == userName);

            user.Profile = user.Profile ?? new UserProfile();
            user.Profile.AllowedCalories = allowedCalories;

            var days = (dateTo.Value - dateFrom.Value).TotalDays;

            for (int i = 0; i < days; i++)
            {
                var date = dateFrom.Value.AddDays(i);
                RandomizeMeal(user, 1, date, breakfasts);
                RandomizeMeal(user, .4, date, forenoonSnacks);
                RandomizeMeal(user, 1, date, lunch);
                RandomizeMeal(user, .8, date, afternoonSnacks);
                RandomizeMeal(user, 1, date, dinner);
                RandomizeMeal(user, .4, date, secondDinner);
            }

        }

        public void Seed()
        {
            SeedInternal().GetAwaiter().GetResult();
        }

        public async Task SeedInternal()
        {
            //
            // DB creation
            if (hostEnvironment.IsEnvironment("Test"))
                context.Database.EnsureDeleted();

            context.Database.Migrate();

            //
            // Seed
            await EnsureRole(RoleConsts.Admin);
            await EnsureRole(RoleConsts.Manager);

            await EnsureUserWithRoles("admin", "admin123$", RoleConsts.Admin, RoleConsts.Manager);
            await EnsureUserWithRoles("manager", "manager123$", RoleConsts.Manager);
            await EnsureUserWithRoles("paul", "paul123$");
            await EnsureUserWithRoles("saul", "saul123$");

            //
            // Conditional
            var cnt = context.Set<User>()
                .Include(u => u.Meals)
                .Where(u=> u.UserName == "saul")
                .SelectMany(u => u.Meals)
                .Count();

            if (cnt < 10)
            {
                await EnsureDataForUser(
                    "saul",
                    dateFrom: DateTime.Now.AddDays(-35),
                    dateTo: DateTime.Now
                    );
            }

            //
            // Finalize
            context.SaveChanges();
        }
    }
}
