using AspCoreBase.Data;
using AspCoreBase.Data.Entities.Authority;
using AspCoreBase.Services;
using AspCoreBase.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AspCoreBase
{
    public class Startup
    {
        private readonly IConfiguration config;
        private readonly IHostEnvironment environment;

        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            config = configuration;
            this.environment = environment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();


            services.AddAutoMapper(typeof(Startup));

            services.AddCors(options => { options.AddPolicy("CorsPolicy", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()); });

            //Configure Identity. IdentityRole = in case we want to configure roles, its a type of data that can store info about a user,
            //but we can derive IdentityRole to have our own role class (but leaving it as IdentityRole for now)
            services.AddIdentity<AuthorityUser, IdentityRole>(cfg =>
            {   //this part of the configuration allows us to make decision about different parts of the system
                cfg.User.RequireUniqueEmail = true;//avoids duplicate emails since it should be a unique identification material anyway
                //cfg.Password.RequireDigit = true;
                //cfg.Password.RequireUppercase = true;
                //cfg.Password.RequireLowercase = true;

            }).AddEntityFrameworkStores<AuthorityDbContext>();   //add this to specify where the data shuld be coming from. We want to explicitly say what type of context to use internally in Identity when it wants to get at objects stored in the Database.

            //by default, when you add Identity, you are supporting authentication based on cookies. Here we are now defining two kinds of authenitcation that we're going to support
            // once implemented,
            services.AddAuthentication(cfg =>
            {
                cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            //.AddCookie()  //do NOT think we need this since we are using JWT
                .AddJwtBearer(cfg =>
                {
                    //cfg.RequireHttpsMetadata = false;
                    //cfg.SaveToken = true;

                    //we need to tell this method about the bearer token that is created in 'CreateToken' method of Account Controller
                    //need to set up the token validation parameters
                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        //// Ensure the token was issued by a trusted authorization server (default true):
                        ValidIssuer = config["Tokens:Issuer"],
                        // Ensure the token audience matches our audience value (default true):
                        ValidateAudience = true,
                        ValidAudience = config["Tokens:Audience"],

                        //// Specify the key used to sign the token:
                        //RequireSignedTokens = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Tokens:Key"])),

                        //// Ensure the token hasn't expired:
                        //RequireExpirationTime = true,
                        //ValidateLifetime = true,

                        //// Clock skew compensates for server time drift.
                        //// We recommend 5 minutes or less:
                        //ClockSkew = System.TimeSpan.FromMinutes(5),
                    };
                    //cfg.Authority = "{yourAuthorizationServerAddress}";   //possibly useful moving forward
                    //cfg.Audience = "{yourAudience}";  //possibly useful moving forward
                });

            /*  Examples of Service implementation types adn DIFFERENCES  */
            //services.AddTransient<IMailService, MailService>();   //light-weight and used only when needed
            //services.AddScoped<IMailService, MailService>();  //heavier with the service being elongated for a longer period
            //services.AddSingleton<IMailService, MailService>();   // heaviest as it will be sustained in the application for the entirety of the application running

            //creating and configuring the database provider using this AspCoreBaseDbContext
            services.AddDbContext<VillageDbContext>(cfg =>
            {
                cfg.UseSqlServer(this.config.GetConnectionString("aspCoreBaseConnectionString"));
            });

            services.AddDbContext<AuthorityDbContext>(cfg =>
            {
                cfg.UseSqlServer(this.config.GetConnectionString("aspCoreBaseAuthorityConnectionString"));
            });

            #region SERVICES
            //example of Service using Dependency inject such that, when it is to be used, the 'services' logic will handle how to create it's 'MailService'
            //example using scoped because the repository should be shared within one scope, usually a request. this way they are not getting constrcuted over and over again
            services.AddScoped<IVillageDbRepository, VillageDbRepository>();
            services.AddScoped<IAuthorityDbRepository, AuthorityDbRepository>();
            services.AddTransient<IAuthenticateService, AuthenticateService>();
            services.AddTransient<IMailService, MailService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IPropertyService, PropertyService>(); // Add IAspCoreBaseDbRepository as a service people can use, but use as the implementation AspCoreBaseDbRepository. perhaps useful in testing
            #endregion

            //// adds all the services that the subsystem requires to run ASP.NET MVC and would get rid of an 'IServiceCollection.AddMvc' error
            //services.AddMvc(opt =>
            //{
            //    if (environment.IsProduction())
            //    {
            //        opt.Filters.Add(new RequireHttpsAttribute());  //  it can also be used inside Controllers or Actions to require https on only certain controllers or actions. This applies it to EVERYWHERE on the site
            //    }
            //    //}).AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //}).AddNewtonsoftJson(options =>
            //{
            //    options.SerializerSettings = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            //});

            //// In production, the Angular files will be served from where the RootPath is set from (below)
            //services.AddSpaStaticFiles(configuration =>
            //{
            //    configuration.RootPath = "[somewhere]/dist";
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); }
            app.UseCors("CorsPolicy");
            app.UseStaticFiles();//no longer using this as it is replaced by the 'Views' folder and its information (copy/pasted the content into the Views folder)
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}

#region IDENTITY SERVER/SWAGGER REVISION VERSION (STILL NEEDS WORK)
//using AspCoreBase.Data;
//using AspCoreBase.Data.Entities.Authority;
//using AspCoreBase.Services;
//using AspCoreBase.Services.Interfaces;
//using AutoMapper;
//using Humanizer;
//using IdentityServer4.AccessTokenValidation;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc.Authorization;
//using Microsoft.AspNetCore.Mvc.ModelBinding;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Serialization;
//using Swashbuckle.AspNetCore.SwaggerGen;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace AspCoreBase
//{    
//    /// <summary>
//    /// This class NEEDS to be cleaned up. We have JWT Bearer process mixed in with OAuth 2.0 tactics inside this entire file (not just this abstract class).
//    /// "Startup_jwtVersion" is the version that has ONLY the jwt stuff with the username/password intended in the viewmodel that handles that process.
//    /// Decisions NEED to be made to determine WHICH authroization process to go with. For now, we're going with JWT Bearer token but with Identity Server
//    /// an OAuth 2.0 things within this that can be accessed with Swagger.
//    /// </summary>
//    public abstract class Startup
//    {
//        public IConfiguration Config { get; }
//        //private readonly IHostEnvironment environment;

//        public Startup(IConfiguration configuration)
//        //public Startup(IConfiguration configuration, IHostEnvironment environment)
//        {
//            Config = configuration;
//            //this.environment = environment;
//        }

//        // This method gets called by the runtime. Use this method to add services to the container.
//        public virtual void ConfigureServices(IServiceCollection services)
//        {
//            services.AddControllers();

//            services.AddCustomSwagger(Config);

//            services.AddAutoMapper(typeof(Startup));

//            services.AddCors(options => { options.AddPolicy("CorsPolicy", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()); });

//            //Configure Identity. IdentityRole = in case we want to configure roles, its a type of data that can store info about a user,
//            //but we can derive IdentityRole to have our own role class (but leaving it as IdentityRole for now)
//            services.AddIdentity<AuthorityUser, IdentityRole>(cfg =>
//            {   //this part of the configuration allows us to make decision about different parts of the system
//                cfg.User.RequireUniqueEmail = true;//avoids duplicate emails since it should be a unique identification material anyway
//                //cfg.Password.RequireDigit = true;
//                //cfg.Password.RequireUppercase = true;
//                //cfg.Password.RequireLowercase = true;

//            }).AddEntityFrameworkStores<AuthorityDbContext>();   //add this to specify where the data shuld be coming from. We want to explicitly say what type of context to use internally in Identity when it wants to get at objects stored in the Database.

//            //by default, when you add Identity, you are supporting authentication based on cookies. Here we are now defining two kinds of authenitcation that we're going to support
//            // once implemented,
//            services.AddAuthentication(cfg =>
//            {
//                cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//                cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//            })
//            //.AddCookie()  //do NOT think we need this since we are using JWT
//                .AddJwtBearer(cfg =>
//                {
//                    //cfg.RequireHttpsMetadata = false;
//                    //cfg.SaveToken = true;

//                    //we need to tell this method about the bearer token that is created in 'CreateToken' method of Account Controller
//                    //need to set up the token validation parameters
//                    cfg.TokenValidationParameters = new TokenValidationParameters()
//                    {
//                        ValidateIssuer = true,
//                        //// Ensure the token was issued by a trusted authorization server (default true):
//                        ValidIssuer = Config["Tokens:Issuer"],
//                        // Ensure the token audience matches our audience value (default true):
//                        ValidateAudience = true,
//                        ValidAudience = Config["Tokens:Audience"],

//                        //// Specify the key used to sign the token:
//                        //RequireSignedTokens = true,
//                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config["Tokens:Key"])),

//                        //// Ensure the token hasn't expired:
//                        //RequireExpirationTime = true,
//                        //ValidateLifetime = true,

//                        //// Clock skew compensates for server time drift.
//                        //// We recommend 5 minutes or less:
//                        //ClockSkew = System.TimeSpan.FromMinutes(5),
//                    };
//                    //cfg.Authority = "{yourAuthorizationServerAddress}";   //possibly useful moving forward
//                    //cfg.Audience = "{yourAudience}";  //possibly useful moving forward
//                });

//            /*  Examples of Service implementation types adn DIFFERENCES  */
//            //services.AddTransient<IMailService, MailService>();   //light-weight and used only when needed
//            //services.AddScoped<IMailService, MailService>();  //heavier with the service being elongated for a longer period
//            //services.AddSingleton<IMailService, MailService>();   // heaviest as it will be sustained in the application for the entirety of the application running

//            //creating and configuring the database provider using this AspCoreBaseDbContext
//            services.AddDbContext<VillageDbContext>(cfg =>
//            {
//                cfg.UseSqlServer(this.Config.GetConnectionString("aspCoreBaseConnectionString"));
//            });

//            services.AddDbContext<AuthorityDbContext>(cfg =>
//            {
//                cfg.UseSqlServer(this.Config.GetConnectionString("aspCoreBaseAuthorityConnectionString"));
//            });

//            #region SERVICES
//            //example of Service using Dependency inject such that, when it is to be used, the 'services' logic will handle how to create it's 'MailService'
//            //example using scoped because the repository should be shared within one scope, usually a request. this way they are not getting constrcuted over and over again
//            services.AddScoped<IVillageDbRepository, VillageDbRepository>();
//            services.AddScoped<IAuthorityDbRepository, AuthorityDbRepository>();
//            services.AddTransient<IAuthenticateService, AuthenticateService>();
//            services.AddTransient<IMailService, MailService>();
//            services.AddTransient<IUserService, UserService>();
//            services.AddTransient<IPropertyService, PropertyService>(); // Add IAspCoreBaseDbRepository as a service people can use, but use as the implementation AspCoreBaseDbRepository. perhaps useful in testing
//            #endregion

//            //// adds all the services that the subsystem requires to run ASP.NET MVC and would get rid of an 'IServiceCollection.AddMvc' error
//            //services.AddMvc(opt =>
//            //{
//            //    if (environment.IsProduction())
//            //    {
//            //        opt.Filters.Add(new RequireHttpsAttribute());  //  it can also be used inside Controllers or Actions to require https on only certain controllers or actions. This applies it to EVERYWHERE on the site
//            //    }
//            //    //}).AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
//            //}).AddNewtonsoftJson(options =>
//            //{
//            //    options.SerializerSettings = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
//            //});

//            //// In production, the Angular files will be served from where the RootPath is set from (below)
//            //services.AddSpaStaticFiles(configuration =>
//            //{
//            //    configuration.RootPath = "[somewhere]/dist";
//            //});

//            services.AddSwaggerGen();
//        }

//        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
//        public virtual void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
//        {
//            if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); }
//            app.UseSwagger();   // Enable middleware to serve generated Swagger as a JSON endpoint.
//            //app.UseSwagger(c => // Use this version of the "app.UseSwagger()" method for reasons commented about below
//            //{//Swashbuckle generates and exposes Swagger JSON in version 3.0 of the specification—officially called the OpenAPI Specification. To support backwards compatibility, you can opt into exposing JSON in the 2.0 format instead
//            //    c.SerializeAsV2 = true;
//            //});
//            app.UseSwaggerUI(c =>   // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
//            {
//                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiCoreBase .NET Core App"); // specifying the Swagger JSON endpoint.
//                c.RoutePrefix = string.Empty;
//            });
//            app.UseCors("CorsPolicy");
//            app.UseStaticFiles();//no longer using this as it is replaced by the 'Views' folder and its information (copy/pasted the content into the Views folder)
//            app.UseAuthentication();
//            app.UseHttpsRedirection();
//            app.UseRouting();
//            app.UseAuthorization();
//            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
//        }
//        public virtual void BindIdentityServerAuthenticationOptions(IdentityServerAuthenticationOptions options)
//            => Config.GetSection("IdentityProvider").Bind(options);
//    }

//    #region AUXILLIARY STARTUP HELPER CLASSES
//    /// <summary>
//    /// I MAY WANT TO REFACTOR THIS INTO ANOTHER CLASS FILE
//    /// </summary>
//    internal static class CustomExtensions
//    {
//        public static IServiceCollection AddCustomMvc(this IServiceCollection services)
//        {
//            services.AddControllers(config =>
//            {
//                var policy = new AuthorizationPolicyBuilder()
//                    .RequireAuthenticatedUser()
//                    .Build();
//                config.Filters.Add(new AuthorizeFilter(policy));
//                config.ValueProviderFactories.Add(new SnakeCaseQueryStringValueProvider());
//            })
//            .AddNewtonsoftJson(options =>
//            {
//                options.SerializerSettings.ContractResolver =
//                new DefaultContractResolver()
//                {
//                    NamingStrategy = new SnakeCaseNamingStrategy(true, false)
//                };
//                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
//            });
//            return services;
//        }

//        /// <summary>
//        /// Currently not correclty implemented and is wrongfully connected to the login/tokenization process, but this can be fixed, rather easily, later
//        /// </summary>
//        /// <param name="services"></param>
//        /// <param name="configuration"></param>
//        /// <returns></returns>
//        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
//        {
//            var authorityUrl = new Uri($"{configuration["IdentityProvider:Authority"]}", UriKind.Absolute);

//            //services.AddSwaggerGen(c =>
//            //{
//            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "APICoreBase API", Version = "v1" });
//            //    c.AddSecurityDefinition("oauth2",
//            //        new OpenApiSecurityScheme
//            //        {
//            //            Description = "Requests an authorization token from Identity Provider",
//            //            Type = SecuritySchemeType.OAuth2,
//            //            Flows = new OpenApiOAuthFlows()
//            //            {
//            //                ClientCredentials = new OpenApiOAuthFlow
//            //                {
//            //                    AuthorizationUrl = authorityUrl,
//            //                    TokenUrl = new Uri(authorityUrl, "/connect/token"),
//            //                    //TokenUrl = new Uri(authorityUrl, "/Account/login"),
//            //                    Scopes = new Dictionary<string, string> {
//            //                        { configuration["IdentityProvider:ApiName"], "APICoreBaseScope" }
//            //                    },
//            //                }
//            //            },
//            //        });
//            //    c.OperationFilter<SwaggerOAuthFilter>();
//            //});

//            services.AddSwaggerGen(c =>
//            {
//                c.SwaggerDoc("v1", new OpenApiInfo { Title = "APICoreBase API", Version = "v1" });
//                c.AddSecurityDefinition("Bearer",
//                    new OpenApiSecurityScheme
//                    {
//                        In = ParameterLocation.Header,
//                        Description = "Use the '/Account/login' endpoint, using an actual user, to generate a token in the response. Enter it as 'Bearer [long token value]' inside this text box",
//                        Name = "Authorization",
//                        Type = SecuritySchemeType.ApiKey
//                    });
//                c.AddSecurityRequirement(new OpenApiSecurityRequirement { {
//                        new OpenApiSecurityScheme{
//                            Reference = new OpenApiReference{
//                                Type = ReferenceType.SecurityScheme,
//                                Id = "Bearer"
//                            }
//                        },
//                        new string[]{}
//                    }
//                });
//            });

//            services.AddSwaggerGenNewtonsoftSupport();

//            return services;
//        }

//        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, Action<IdentityServerAuthenticationOptions> configureOptions)
//        {
//            services
//                .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
//                .AddCookie("none")
//                .AddIdentityServerAuthentication(configureOptions);
//            return services;
//        }
//    }

//    /// <summary>
//    /// I MAY WANT TO REFACTOR THIS INTO ANOTHER CLASS FILE
//    /// </summary>
//    public class SnakeCaseQueryStringValueProvider : IValueProviderFactory
//    {
//        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
//        {
//            if (context == null)
//            {
//                throw new ArgumentNullException(nameof(context));
//            }

//            context.ValueProviders.Add(BuildQueryStringValueProvider(context));

//            return Task.CompletedTask;
//        }

//        private static QueryStringValueProvider BuildQueryStringValueProvider(ValueProviderFactoryContext context)
//        {
//            var collection = context.ActionContext.HttpContext.Request.Query
//                .ToDictionary(t => t.Key.Pascalize(), t => t.Value, StringComparer.OrdinalIgnoreCase);

//            var queryStringValueProvider = new QueryStringValueProvider(
//                BindingSource.Query,
//                new QueryCollection(collection),
//                CultureInfo.InvariantCulture);

//            return queryStringValueProvider;
//        }
//    }

//    /// <summary>
//    ///     Updates swagger.json output to include security information
//    ///     for any controllers or actions that contain the Authorize attribute
//    /// </summary>
//    public class SwaggerOAuthFilter : IOperationFilter
//    {
//        public void Apply(OpenApiOperation operation, OperationFilterContext context)
//        {
//            // Check for authorize filter.
//            var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
//            var isAuthorized = filterPipeline.Select(filterInfo => filterInfo.Filter).Any(filter => filter is AuthorizeFilter);

//            if (!isAuthorized) return;

//            var oAuthScheme = new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
//            };

//            operation.Security = new List<OpenApiSecurityRequirement>
//                {
//                    new OpenApiSecurityRequirement
//                    {
//                        [oAuthScheme ] = new string[0]
//                    }
//                };
//        }
//    }
//    #endregion

//}
#endregion

#region PURE REFERENCE VERSION
//using AutoMapper;
//using Humanizer;
//using IdentityServer4.AccessTokenValidation;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.HttpOverrides;
//using Microsoft.AspNetCore.Mvc.Authorization;
//using Microsoft.AspNetCore.Mvc.ModelBinding;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.OpenApi.Models;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Serialization;
//using Swashbuckle.AspNetCore.SwaggerGen;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Threading.Tasks;

//namespace AceNeogovApi
//{
//    public abstract class Startup
//    {
//        public IConfiguration Configuration { get; }

//        protected Startup(IConfiguration configuration)
//        {
//            Configuration = configuration;
//        }

//        // This method gets called by the runtime. Use this method to add services to the container.
//        public virtual void ConfigureServices(IServiceCollection services)
//        {
//            services
//                .AddCustomMvc()
//                .AddCustomSwagger(Configuration)
//                .AddCustomAuthentication(BindIdentityServerAuthenticationOptions)
//                //.AddCorrelationIdAccessor()
//                //.AddLocaleService()
//                //.AddRequestResponseLoggingDefaultServices(Configuration)
//                .AddDistributedMemoryCache()
//                .AddAutoMapper(typeof(Startup))
//                .AddHealthChecks();
//        }

//        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
//        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
//        {
//            app.UseHealthChecks("/healthcheck");

//            //app.UseApiExceptionMiddleware();

//            //app.UseRequestResponseLoggingMiddleware();

//            app.UseRouting();

//            app.UseAuthentication();

//            app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedProto });

//            // Shows UseCors with CorsPolicyBuilder.
//            app.UseCors(builder => builder.AllowAnyMethod().AllowAnyHeader());

//            //app.UseCorrelationIdMiddleware();

//            // Enable middleware to serve generated Swagger as a JSON endpoint
//            app.UseSwagger();

//            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
//            app.UseSwaggerUI(c =>
//            {
//                c.SwaggerEndpoint("/swagger/v1/swagger.json", "NeoGov API");
//                c.RoutePrefix = string.Empty;
//            });
//            app.UseEndpoints(config =>
//            {
//                config.MapControllers();
//            });
//        }

//        public virtual void BindIdentityServerAuthenticationOptions(IdentityServerAuthenticationOptions options)
//            => Configuration.GetSection("IdentityProvider").Bind(options);
//    }

//    internal static class CustomExtensions
//    {
//        public static IServiceCollection AddCustomMvc(this IServiceCollection services)
//        {
//            services.AddControllers(config =>
//            {
//                var policy = new AuthorizationPolicyBuilder()
//                    .RequireAuthenticatedUser()
//                    .Build();
//                config.Filters.Add(new AuthorizeFilter(policy));
//                config.ValueProviderFactories.Add(new SnakeCaseQueryStringValueProvider());
//            })
//            .AddNewtonsoftJson(options =>
//            {
//                options.SerializerSettings.ContractResolver =
//                new DefaultContractResolver()
//                {
//                    NamingStrategy = new SnakeCaseNamingStrategy(true, false)
//                };
//                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
//            });
//            return services;
//        }

//        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
//        {
//            var authorityUrl = new Uri($"{configuration["IdentityProvider:Authority"]}", UriKind.Absolute);
//            services.AddSwaggerGen(c =>
//            {
//                c.SwaggerDoc("v1", new OpenApiInfo { Title = "NeoGov API", Version = "v1" });
//                c.AddSecurityDefinition("oauth2",
//                    new OpenApiSecurityScheme
//                    {
//                        Description = "Requests an authorization token from Identity Provider",
//                        Type = SecuritySchemeType.OAuth2,
//                        Flows = new OpenApiOAuthFlows()
//                        {
//                            ClientCredentials = new OpenApiOAuthFlow
//                            {
//                                AuthorizationUrl = authorityUrl,
//                                TokenUrl = new Uri(authorityUrl, "/connect/token"),
//                                Scopes = new Dictionary<string, string> {
//                                    { configuration["IdentityProvider:ApiName"], "AceNeogovApi" }
//                                },
//                            }
//                        },
//                    });
//                c.OperationFilter<SwaggerOAuthFilter>();
//            });
//            services.AddSwaggerGenNewtonsoftSupport();
//            return services;
//        }

//        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, Action<IdentityServerAuthenticationOptions> configureOptions)
//        {
//            services
//                .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
//                .AddCookie("none")
//                .AddIdentityServerAuthentication(configureOptions);
//            return services;
//        }
//    }

//    /// <summary>
//    /// I MAY WANT TO REFACTOR THIS INTO ANOTHER CLASS FILE
//    /// </summary>
//    public class SnakeCaseQueryStringValueProvider : IValueProviderFactory
//    {
//        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
//        {
//            if (context == null)
//            {
//                throw new ArgumentNullException(nameof(context));
//            }

//            context.ValueProviders.Add(BuildQueryStringValueProvider(context));

//            return Task.CompletedTask;
//        }

//        private static QueryStringValueProvider BuildQueryStringValueProvider(ValueProviderFactoryContext context)
//        {
//            var collection = context.ActionContext.HttpContext.Request.Query
//                .ToDictionary(t => t.Key.Pascalize(), t => t.Value, StringComparer.OrdinalIgnoreCase);

//            var queryStringValueProvider = new QueryStringValueProvider(
//                BindingSource.Query,
//                new QueryCollection(collection),
//                CultureInfo.InvariantCulture);

//            return queryStringValueProvider;
//        }
//    }

//    /// <summary>
//    ///     Updates swagger.json output to include security information
//    ///     for any controllers or actions that contain the Authorize attribute
//    /// </summary>
//    public class SwaggerOAuthFilter : IOperationFilter
//    {
//        public void Apply(OpenApiOperation operation, OperationFilterContext context)
//        {
//            // Check for authorize filter.
//            var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
//            var isAuthorized = filterPipeline.Select(filterInfo => filterInfo.Filter).Any(filter => filter is AuthorizeFilter);

//            if (!isAuthorized) return;

//            var oAuthScheme = new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
//            };

//            operation.Security = new List<OpenApiSecurityRequirement>
//                {
//                    new OpenApiSecurityRequirement
//                    {
//                        [oAuthScheme ] = new string[0]
//                    }
//                };
//        }
//    }
//}
#endregion