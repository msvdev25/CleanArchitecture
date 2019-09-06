using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace MFS.Repo
{
    public sealed class DependencyHandler
    {
        private static bool IsDependencyRegisted;

        private DependencyHandler()
        {
            IsDependencyRegisted = false;
        }

        public static void Configure(IServiceCollection services)
        {
            if (!IsDependencyRegisted)
            {
                services.AddScoped<ITenantHandler, TenantHandler>();

                Assembly assembly = Assembly.LoadFile(Assembly.GetExecutingAssembly().Location);

                foreach (var type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Repository")))
                    services.Add(new ServiceDescriptor(type.GetInterfaces().First(i => i.Name.EndsWith(type.Name)), type, ServiceLifetime.Scoped));

                IsDependencyRegisted = true;
            }
        }

    }
}
