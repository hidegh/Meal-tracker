using System;
using System.Collections.Generic;

namespace RMealsAPI.Features.Meals
{
    public class MealDailySummaryDto
    {
        public DateTime Day { get; internal set; }
        public int MealsCount { get; internal set; }
        public int DailyCaloriesConsumed { get; internal set; }
        public bool DailyCaloriesExceeded { get; internal set; }
        public IEnumerable<MealDailyItemDto> Meals { get; set; }

        public class MealDailyItemDto
        {
            public long? Id { get; internal set; }
            public DateTime Date { get; internal set; }
            public int Calories { get; internal set; }
            public string Description { get; internal set; }
        }
    }
}
