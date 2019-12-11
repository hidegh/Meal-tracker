using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RMealsAPI.Model;

namespace RMealsAPI.Persistence.Mapping
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasOne(r => r.Profile).WithMany(); // NOTE: EF vs NH - https://stackoverflow.com/questions/20757594/ef-codefirst-should-i-initialize-navigation-properties

            builder.HasMany(r => r.Meals).WithOne(r => r.User)
                .IsRequired(); // NOTE: this is what removing meal from user colection makes the item to be deleted (and not just unassigned)                
        }

    }
}
