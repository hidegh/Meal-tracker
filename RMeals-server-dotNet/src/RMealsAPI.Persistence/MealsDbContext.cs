using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RMealsAPI.Persistence.Mapping;
using RMealsAPI.Model;
using System;
using RMealsAPI.Persistence;
using System.Threading.Tasks;

namespace NetCore3.Persistence
{
    public class MealsDbContext : IdentityDbContext<User, IdentityRole<long>, long>
    {

        public MealsDbContext(): base()
        {

        }

        public void Reload<TEntity>(TEntity entity) where TEntity : class
        {
            Entry(entity).Reload();
        }
        
        public Task ReloadAsync<TEntity>(TEntity entity) where TEntity : class
        {
            return Entry(entity).ReloadAsync();
        }

        /// <summary>
        /// Needed for EntityFrameworkCore\Add-Migration InitialCreate
        /// </summary>
        /// <param name="options"></param>
        public MealsDbContext(DbContextOptions<MealsDbContext> options): base(options)
        {
 
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            // NOTE:
            // lazy loading not, but proxies are a must
            // we must be able to distinguish between a NULL value and a not loaded property ref.
            // unfortunatelly without proxies we get a falsy NULL
            //
            // so rather allow proxies, do usual eager load and eventuelly run into an issue with an additional select (profiling with ORM is a must)...
            // ...than to falsly interpret a value as NULL, just because it was not loaded...and no non-loaded exception is thrown by a non existing proxy
            // builder.UseLazyLoadingProxies();
            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Console.WriteLine(">>> on model creating executing...");
            base.OnModelCreating(modelBuilder);           
            modelBuilder.RegisterMappingsInsideNamespace<IMappingContainer>();
            Console.WriteLine(">>> on model creating executed...");

            // User.Roles
            // https://entityframeworkcore.com/knowledge-base/51004516/-net-core-2-1-identity-get-all-users-with-their-associated-roles
            /*
            builder.Entity<ApplicationUserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });
            */
        }

    }
}
