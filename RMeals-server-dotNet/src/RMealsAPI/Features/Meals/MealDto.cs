using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RMealsAPI.Features.Meals
{

    public class MealDto
    {
        [DisplayName("Meal time (and date)")]
        [Required(ErrorMessage = "'{0}' must be set")]
        public DateTime Date { get; set; }

        [DisplayName("Calories")]
        [Required(ErrorMessage = "'{0}' must be set")]
        [Range(0, 5000, ErrorMessage = "Value of '{0}' must be between {1} and {2}")]
        public int Calories { get; set; }

        [DisplayName("Meal (description)")]
        public string Description { get; set; }

    }
}
