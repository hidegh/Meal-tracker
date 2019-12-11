using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RMealsAPI.Model;

namespace RMealsAPI.Persistence.Mapping
{
    public class UserMealMap : IEntityTypeConfiguration<UserMeal>
    {
        public void Configure(EntityTypeBuilder<UserMeal> builder)
        {
            
        }
    }
}
