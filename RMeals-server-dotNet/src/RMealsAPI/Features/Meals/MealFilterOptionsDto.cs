using System;

namespace RMealsAPI.Features.Meals
{
    public class MealFilterOptionsDto
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public TimeSpan? TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
    }
}
