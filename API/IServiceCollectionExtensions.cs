using System.Reflection;

namespace API
{
    public static class IServiceCollectionExtensions
    {
        private static bool IsAssignableToGeneric(Type type, Type genericType)
        {
            return type.GetInterfaces()
                .Where(i => i.IsGenericType)
                .Any(i => i.GetGenericTypeDefinition() == genericType);
        }

        public static void AddRepositories(this IServiceCollection services, Assembly assembly)
        {
            var repositoryTypes = assembly.GetTypes()
            .Where(type => !type.IsAbstract && !type.IsInterface && IsAssignableToGeneric(type, typeof(IGenericRepository<>)));

            // Register each repository type as scoped service
            foreach (var repositoryType in repositoryTypes)
            {
                services.AddScoped(repositoryType);
            }
            //services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
        
    }
}
