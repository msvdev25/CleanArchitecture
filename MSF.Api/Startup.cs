using Core.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MSF.Data;
using MSF.Domain;
using MSF.Service;
using System.Text;

namespace MSF.Api
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Add framework services.
            services.AddOptions();

            // Unit of work for Data operations via Core.Data library.
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Authentication/Authorization configuration.
            ConfigureAuthentication(services);

            // Resolve the service DB Context dependencies.
            services.ConfigureDataContext(Configuration);
            
            // Resolve the service dependencies.
            services.ConfigureServices();

            // Swagger configuration.
             services.AddOpenApiDocument(document => { document.DocumentName = "Open Api"; });
            services.AddLogging(l => l.AddEventSourceLogger());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            app.UseHsts();

            //Cors
            app.UseCors(builder =>
            {
                builder.AllowAnyHeader();
                builder.AllowCredentials();

                // For any origin access.
                //builder.AllowAnyOrigin(); 

                // Enable request for specific origin (ULR address) based on config file.
                builder.WithOrigins(Configuration.GetSection("AllowdOrigins").Get<string[]>());

                // Enable for specific type (GET,POST) based on config file.
                builder.WithMethods(Configuration.GetSection("AlloudMethods").Get<string[]>());

            });

            app.UseAuthentication();
            app.UseMvc();

            // Seagger documentation.
            app.UseOpenApi(); // serve documents
            app.UseSwaggerUi3(s => { s.WithCredentials = true; } ); // serve Swagger UI

        }

        private void ConfigureAuthentication(IServiceCollection services)
        {
            // JWT token authentication.
            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(op =>
            {
                op.RequireHttpsMetadata = false;
                op.SaveToken = true;
                op.IncludeErrorDetails = true;
                op.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    //ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidIssuers = Configuration.GetSection("Jwt:Issueres").Get<string[]>(),
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                    ClockSkew = System.TimeSpan.Zero // Remove delay of token when expire                
                };
            });

            //Authorization [policy based]
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy(Constants.AdminAccess, p => { p.RequireRole(Role.Admin.ToString()); p.RequireAuthenticatedUser(); });
                auth.AddPolicy(Constants.AddEditDeleteAccess, p => { p.RequireRole(Role.Admin.ToString(), Role.AddEditDelete.ToString()); p.RequireAuthenticatedUser(); });
                auth.AddPolicy(Constants.AddEditAccess, p => { p.RequireRole(Role.AddEdit.ToString(), Role.Admin.ToString(), Role.AddEditDelete.ToString() ); p.RequireAuthenticatedUser(); });
                auth.AddPolicy(Constants.ReadOnlyAccess, p => { p.RequireAuthenticatedUser(); });
            });
        }
    }
}
