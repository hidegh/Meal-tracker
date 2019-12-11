using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace RMealsAPI.Model
{
    public class User : IdentityUser<long>
    {
        public virtual UserProfile Profile { get; set; }

        public virtual ICollection<UserMeal> Meals {get; set;}

        public User()
        {
            // NOTE:
            // https://stackoverflow.com/questions/20757594/ef-codefirst-should-i-initialize-navigation-properties
            //
            // While creating a N:1 dependency here does a mess (won't get saved if not reassigned),
            // Profile = new UserProfile();
            // ...the creation of internal collections is a good practice (needed at initialization and seed due the way how User is created inside identity and user manager)!
            Meals = new List<UserMeal>();
        }
    }
}
