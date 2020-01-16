using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.UriParser;
using NetCore3.Persistence;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NSwag.AspNetCore;
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

            // API versioning
            services.AddApiVersioning(o =>
            {
                o.ApiVersionReader = new UrlSegmentApiVersionReader();
                // NOTE: def. version won't wotk with url segments (at least not out of the box)
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.SubstituteApiVersionInUrl = true;
            });

            // OData setup (we can use any of the camelCase variant)
            services.TryAddSingleton(_ => new ODataConventionModelBuilder(_, true).EnableLowerCamelCase());
            services.TryAddSingleton(_ => new ODataUriResolver() { EnableCaseInsensitive = true });
            services.AddOData();

            // Originally a workaround: https://github.com/OData/WebApi/issues/1177...
            //
            // ...but from .NET Core 3.x it's a solution for swagger, where we:got this error:
            // InvalidOperationException: No media types found in 'Microsoft.AspNet.OData.Formatter.ODataOutputFormatter.SupportedMediaTypes'. Add at least one media type to the list of supported media types.
            // ...when hitting: https://localhost:44383/swagger/v1/swagger.json
            services.AddMvcCore(options =>
			{
				foreach (var outputFormatter in options.OutputFormatters.OfType<ODataOutputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
					outputFormatter.SupportedMediaTypes.Add(new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
				foreach (var inputFormatter in options.InputFormatters.OfType<ODataInputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
					inputFormatter.SupportedMediaTypes.Add(new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
			});

            services
                .AddMvc(options =>
                {
                    options.EnableEndpointRouting = false;

                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();

                    options.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddFeatureFolders();

            // NSwag (build an intermediate service provider & resolve IApiVersionDescriptionProvider)
            var apiVersionDescriptionProvider = services.BuildServiceProvider().GetService<IApiVersionDescriptionProvider>();

            // build a swagger endpoint for each discovered API version
            foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
                var versionGroup = description.GroupName;

                services
                    .AddOpenApiDocument(document =>
                    {
                        // Title, name, version...
                        document.Title = "RMeals API";

                        // DocumentName is used in the top-right corner of the UI, it's also part of the swagger route
                        document.DocumentName = versionGroup;

                        // ApiGroupNames is used to filter versions (ApiVersionAttribute) that are includes in this swagger document - NOTE: use v1 for 1.0 v1.1 for 1.1 and so on...
                        document.ApiGroupNames = new[] { versionGroup };

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider, ILoggerFactory loggerFactory, MealsDbContextInitializer mealsDbContextInitializer)
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

            app.UseMvc(routes =>
            {
                routes.EnableDependencyInjection();
                routes.Select().Expand().Filter().OrderBy().MaxTop(null).Count();
            });

            // Nswag
            app
                .UseOpenApi()
                .UseSwaggerUi3(options =>
                {
                    // build a swagger endpoint for each discovered API version
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerRoutes.Add(new SwaggerUi3Route($"{description.GroupName}", $"/swagger/{description.GroupName}/swagger.json"));
                    }
                });
        }
    }
}
