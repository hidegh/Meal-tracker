using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RMealsAPI.Features.Users
{
    public class UserProfileDto
    {
        [DisplayName("Allowed calories")]
        [Required(ErrorMessage = "'{0}' must be set")]
        [Range(0, 10000, ErrorMessage = "Value of '{0}' must be between {1} and {2}")]
        public int AllowedCalories { get; set; }
    }
}
