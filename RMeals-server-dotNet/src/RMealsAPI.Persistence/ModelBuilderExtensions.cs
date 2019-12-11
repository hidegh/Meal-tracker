using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace RMealsAPI.Persistence
{
    public static class ModelBuilderExtensions
    {
        public delegate bool MappingFilterDelegate(Type mappingClass, Type entity);

        public static void RegisterMappingsFrom<TAssembly>(this ModelBuilder modelBuilder, MappingFilterDelegate filter = null)
        {
            var mappingsAssembly = typeof(TAssembly);

            var q =
                from type in mappingsAssembly.Assembly.GetTypes()
                from @interface in type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
                let classType = type
                let entityType = @interface.GetGenericArguments()[0]
                where filter == null || filter(classType, entityType)
                select type;

            var entityTypeConfigurationTypesToRegister = q.Distinct().ToList();

            foreach (var entityTypeconfigurationType in entityTypeConfigurationTypesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(entityTypeconfigurationType);
                modelBuilder.ApplyConfiguration(configurationInstance);
            }

        }

        public static void RegisterMappingsInsideNamespace<TMapMarker>(this ModelBuilder modelBuilder, MappingFilterDelegate filter = null)
        {
            var mappingMarker = typeof(TMapMarker);
            var mappingMarkerNS = mappingMarker.Namespace;

            RegisterMappingsFrom<TMapMarker>(modelBuilder, (classType, entityType) =>
            {
                // make sure we're inside the desired namespace
                if (!classType.FullName.StartsWith(mappingMarkerNS + "."))
                    return false;

                // do additional custom filter logic
                return filter == null || filter(classType, entityType);
            });

        }
    }
}
