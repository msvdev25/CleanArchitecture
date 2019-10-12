using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace MSF.Service
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
                services.AddScoped<ICommonService, CommonService>();

                Assembly assembly = Assembly.LoadFile(Assembly.GetExecutingAssembly().Location);

                foreach (var type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service")))
                    services.Add(new ServiceDescriptor(type.GetInterfaces().First(i => i.Name.EndsWith(type.Name)), type, ServiceLifetime.Scoped));

                IsDependencyRegisted = true;

                Repo.DependencyHandler.Configure(services);
            }
        }

    }
}
