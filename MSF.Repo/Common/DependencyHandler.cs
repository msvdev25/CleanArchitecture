using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace MSF.Repo
{
    public static class DependencyHandler
    {

        public static void ConfigureRepository(this IServiceCollection services)
        {
            services.AddScoped<ITenantHandler, TenantHandler>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            //Assembly assembly = Assembly.LoadFile(Assembly.GetExecutingAssembly().Location);

            //foreach (var type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Repository")))
            //    services.Add(new ServiceDescriptor(type.GetInterfaces().First(i => i.Name.EndsWith(type.Name)), type, ServiceLifetime.Scoped));

        }

    }
}
