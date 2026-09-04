using System.Reflection;

namespace FinalProject
{
    public static class IServiceCollectionExtensions
    {
        private static bool IsAssignableToGeneric(Type type, Type genericType)
        {
            return type.GetInterfaces()
                .Where(i => i.IsGenericType)
                .Any(i => i.GetGenericTypeDefinition() == genericType);
        }

        public static void AddClients(this IServiceCollection services, Assembly assembly)
        {
            var clientTypes = assembly.GetTypes()
            .Where(type => !type.IsAbstract && !type.IsInterface && IsAssignableToGeneric(type, typeof(IGenericApiClient<>)));

            // Register each client type as scoped service
            foreach (var clientType in clientTypes)
            {
                services.AddScoped(clientType);
            }
            //services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
