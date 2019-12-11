using System;

namespace RMealsAPI.Model
{
    public class UserMeal
    {
        public virtual long? Id { get; set; }
        public virtual User User { get; set; }

        public virtual DateTime Date { get; set; }
        public virtual string Description { get; set; }
        public virtual int Calories { get; set; }
    }
}
