using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NetCore3.Persistence;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NSwag.Generation.Processors.Security;
using RMealsAPI.Code.Identity;
using RMealsAPI.Model;

namespace RMealsAPI
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
            services.AddSingleton(Configuration);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //
            // DbContext and ASP.NET core Identity
            var connectionString = Configuration["Databases:Main:ConnectionString"];
            var migrationAssembly = typeof(MealsDbContext).GetTypeInfo().Assembly.GetName().Name;
            services.AddDbContext<MealsDbContext>(opt => opt.UseLazyLoadingProxies().UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationAssembly)));
                
            services
                .AddIdentityCore<User>(options =>
                {
                    // NOTE: email and email verification is a must for password retrieval and security (for sample it's turned off)
                    options.User.RequireUniqueEmail = false;
                    options.SignIn.RequireConfirmedEmail = false;
                    options.Tokens.EmailConfirmationTokenProvider = "emailConfirmationTokenProvider";

                    // NOTE: for real app we need more restrictions
                    options.Password.RequiredUniqueChars = 1;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireNonAlphanumeric = false;

                    options.Lockout.AllowedForNewUsers = true;
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    // NOTE: in real env. give at least 5 mins, slows brute-force but still acceptable period for user
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
                })
                .AddRoles<IdentityRole<long>>()
                .AddEntityFrameworkStores<MealsDbContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<EmailConfirmationTokenProvider<User>>("emailConfirmationTokenProvider")
                .AddPasswordValidator<EnchancedPasswordValidator<User>>();

            services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromHours(6));
            services.Configure<EmailConfirmationTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromDays(2));

            // Note: this registers auth-related providers
            services.AddAuthentication();

            // NOTE: register our initializer
            services.AddScoped<MealsDbContextInitializer>();
            
            //
            // ...
            services
                .AddControllers()
                .AddNewtonsoftJson() // NOTE: .NET Core 3's new Json does not haw TimeSpan support
                ;

            services
               .AddAuthentication(options =>
               {
                   options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                   options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
               })
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters()
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,

                       RequireExpirationTime = true,
                       ValidateLifetime = true,

                       ValidateIssuerSigningKey = true,

                       ValidIssuer = Configuration["Jwt:Issuer"],
                       ValidAudience = Configuration["Jwt:Audience"],
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                   };
               });

            services
                .AddMvc(options =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();

                    options.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddFeatureFolders();

            // NSwag
            services
                .AddOpenApiDocument(document => {

                    // Name, version...
                    document.DocumentName = "v1";

                    // NOTE: remove this if no versioning is used
                    // document.ApiGroupNames = new[] { "v1" };

                    // Json serialization
                    document.SerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

                    // Authentication https://github.com/RicoSuter/NSwag/issues/869
                    document.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT token"));
                    document.AddSecurity(
                        "JWT token",
                        Enumerable.Empty<string>(),
                        new NSwag.OpenApiSecurityScheme()
                        {
                            Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
                            Name = nameof(Authorization),
                            In = NSwag.OpenApiSecurityApiKeyLocation.Header,
                            Description = "Copy this into  the value field: \nBearer {my long token}"
                        });
                })
                ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, MealsDbContextInitializer mealsDbContextInitializer)
        {
            mealsDbContextInitializer.Seed();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(cfg => cfg.AllowAnyHeader().AllowAnyMethod().WithOrigins(Configuration["AllowedHosts"]));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Nswag
            app
                .UseOpenApi()
                .UseSwaggerUi3();
        }
    }
}
