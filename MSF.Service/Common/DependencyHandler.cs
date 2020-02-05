using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using MSF.Repo;

namespace MSF.Service
{
    public static class DependencyHandler
    {
        public static void ConfigureServices(this IServiceCollection services)
        {

            // Resolve repository dependencies.
            services.ConfigureRepository();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICategoryService, CategoryService>();

            //Assembly assembly = Assembly.LoadFile(Assembly.GetExecutingAssembly().Location);

            //foreach (var type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service")))
            //    services.Add(new ServiceDescriptor(type.GetInterfaces().First(i => i.Name.EndsWith(type.Name)), type, ServiceLifetime.Scoped));

        }

    }
}
