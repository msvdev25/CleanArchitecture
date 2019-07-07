using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RS2.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cart.Repo
{
    
    public static class DependencyHandler 
    {
        public static void RegisterDependencies( IServiceCollection services)
        {            
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
        }
    }
}
