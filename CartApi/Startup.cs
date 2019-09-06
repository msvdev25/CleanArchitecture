using MFS.Data;
using MFS.Domain;
using MFS.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RS2.Core;
using System.Text;

namespace MFS.Api
{
    public class Startup
    {
        private readonly IHostingEnvironment HostEnv;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            this.HostEnv = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Add framework services.
            services.AddOptions();

            // Identity for authentication.
            services.AddIdentity<AppUser, IdentityRole>(opt =>
            {
                opt.User.RequireUniqueEmail = true;
                opt.Password.RequiredLength = 6;
                opt.Password.RequireNonAlphanumeric = true;
            })
            .AddEntityFrameworkStores<UserContext>()
            .AddDefaultTokenProviders();

            services.AddDbContext<UserContext>(options => options
                .UseSqlServer(Configuration.GetConnectionString("LoginConnection")));

            // Configure Login context. 
            services.AddScoped<IdentityDbContext<AppUser>, UserContext>();

            // Unit of work for Data operations via RS2.Core library.
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddDbContext<TranDbContext>();

            // Configure Transaction DB context.
            services.AddScoped<DbContext, TranDbContext>();

            services.AddScoped<IAppClaimHandler, AppClaimHandler>();

            // Authentication/Authorization configuration.
            ConfigureAuthentication(services);

            // Resolve the service dependencies.
            DependencyHandler.Configure(services);

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
            app.UseSwaggerUi3(); // serve Swagger UI

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
                    ValidIssuers = Configuration.GetSection("Jwt:Issuer").Get<string[]>(),
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                    ClockSkew = System.TimeSpan.Zero // remove delay of token when expire
                };
            });

            //Authorization
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy(Constants.AdminAccess, p => p.RequireRole(Global.RoleList));
                auth.AddPolicy(Constants.AddEditDeleteAccess, p => p.RequireRole(Role.ReadOnly.ToString(), Role.AddEditDelete.ToString(), Role.AddEdit.ToString()));
                auth.AddPolicy(Constants.AddEditAccess, p => p.RequireRole(Role.ReadOnly.ToString(), Role.AddEdit.ToString()));
                auth.AddPolicy(Constants.ReadOnlyAccess, p => p.RequireRole(Role.ReadOnly.ToString()));
            });
        }

    }
}
