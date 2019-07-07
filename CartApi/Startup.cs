using Cart.Data;
using Cart.Domain;
using Cart.Repo;
using Cart.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RS2.Core;
using System.Text;


namespace Cart.Api
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

            // Identity for authentication 
            services.AddIdentity<AppUser, IdentityRole>(opt =>
            {
                opt.User.RequireUniqueEmail = true;
                opt.Password.RequiredLength = 6;
                opt.Password.RequireNonAlphanumeric = true;
            })
            .AddEntityFrameworkStores<LoginContext>()
            .AddDefaultTokenProviders();

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
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                    ClockSkew = System.TimeSpan.Zero // remove delay of token when expire
                };
            });

            //if ( HostEnv.IsDevelopment())
            //{
            //    services.AddDbContext<LoginContext>(options => options
            //        .UseInMemoryDatabase("LoginContext"));

            //    services.AddDbContext<ShoppingContext>(options => options
            //        .UseInMemoryDatabase("MyCart"));

            //}
            //else
            {
                services.AddDbContext<LoginContext>(options => options
                    .UseSqlServer(Configuration.GetConnectionString("LoginConnection")));

                services.AddDbContext<ShoppingContext>(options => options
                    .UseSqlServer(Configuration.GetConnectionString("CartConnection")));
            }
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Configure Transaction and Login DB context.
            services.AddScoped<DbContext, ShoppingContext>();
            services.AddScoped<IdentityDbContext<AppUser>, LoginContext>();

            ConfigureServiceDependencies(services);

            services.AddOpenApiDocument(document => { document.DocumentName = "a"; });
            services.AddSwaggerDocument(document => { document.DocumentName = "b"; });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //Cors
            app.UseCors(builder =>
            {
                builder.AllowAnyHeader();
                builder.AllowCredentials();

                //builder.AllowAnyOrigin(); // For anyone access.

                // Enable request for specific origin (ULR)
                builder.WithOrigins(Configuration.GetSection("AllowdOrigins").Get<string[]>());

                // Enable for specific type (GET,POST)
                builder.WithMethods(Configuration.GetSection("AlloudMethods").Get<string[]>());

            });

            app.UseAuthentication();
            app.UseMvc();

            app.UseOpenApi(); // serve documents
            app.UseSwaggerUi3(); // serve Swagger UI

        }

        private void ConfigureServiceDependencies(IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();

            // Resolve the Repository dependencies.
            DependencyHandler.RegisterDependencies(services);

        }
    }
}
