using AspCoreApiTemplate.Data;
using AspCoreApiTemplate.Data.Entities.Authority;
using AspCoreApiTemplate.Models;
using AspCoreApiTemplate.Services;
using AspCoreApiTemplate.Services.Interfaces;
using AutoMapper;
using Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace AspCoreApiTemplate
{
    public class Startup
    {
        private readonly IConfiguration config; //  appsettings.json < appsettings.{env}.json overrides < User secrets < Environment Variables (launchSettings.json) < Command-line arguments (i.e. from the CLI)
        //private readonly IHostEnvironment environment;

        public Startup(IConfiguration configuration)//, IHostEnvironment environment)
        {
            config = configuration;
            //this.environment = environment;
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

            //creating and configuring the database provider using this AspCoreApiTemplateDbContext. Other databasese, in a real world scenario, would probaby be added such as this "Example" on it's own to support more sufficient MicroServices architecture
            //This Solution mimics a 'gateway' server application since it's still a Template/Base project. Expansion into other added Project within this solution SHOULD have it's own setup, ideally.
            //Above all, things may easily be moved around whenever
            services.AddDbContext<ExampleDbContext>(cfg =>
            {
                cfg.UseSqlServer(this.config.GetConnectionString("AspCoreApiTemplateConnectionString"));
            });

            ////Just another version of the line above. it's better performance, but both work fine. AddDbContextPool can keep multiple DBContext objects alive and gives you an unused one rather than creating a new one each time
            //services.AddDbContextPool<ExampleDbContext>(options => options.UseSqlServer("AspCoreApiTemplateConnectionString"));

            services.AddDbContext<AuthorityDbContext>(cfg =>
            {
                cfg.UseSqlServer(this.config.GetConnectionString("AspCoreApiTemplateAuthorityConnectionString"));
            });

            #region SERVICES
            //example of Service using Dependency inject such that, when it is to be used, the 'services' logic will handle how to create it's 'MailService'
            //example using scoped because the repository should be shared within one scope, usually a request. this way they are not getting constrcuted over and over again
            services.Configure<AppSettings>(config.GetSection("AppSettings"));
            services.AddScoped<IExampleDbRepository, ExampleDbRepository>();
            services.AddScoped<IAuthorityDbRepository, AuthorityDbRepository>(); ;
            services.AddScoped<IErrorHandler, ErrorHandler>();
            services.AddTransient<IAuthenticateService, AuthenticateService>();
            services.AddTransient<IMailService, MailService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IExampleService, ExampleService>(); // Add IAspCoreApiTemplateDbRepository as a service people can use, but use as the implementation AspCoreTemplateDbRepository. perhaps useful in testing
            services.AddScoped(typeof(IMockIdServerVessel<>), typeof(MockIdServerVessel<>));//This is just a TEMPORARY class/process that mimics requests being sent to the back end using Authorization/Authentication. DELETE when no longer needed
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AspCoreApiTemplate", Version = "Asp .NET Core API's Base back end processing API" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            { // should be in the middleware as early as possible as to be able to show anything wrong for any middleware BELOW this implementation
                app.UseDeveloperExceptionPage();//creates an advanced looking exception page
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AspCoreApiTemplate v1"));
            }
            else if (env.IsEnvironment("Made Up Env as the 'ASPNETCORE_ENVIRONMENT' value"))
            {
                //jsut another way of getting the environment
            }

            app.UseCors("CorsPolicy");

            ////EXAMPLES of static files in .NET Core (NOT NEEDED because there are no files to use), use this BEFORE UseStaticFiles() method is used
            ///Default directory for static files is 'wwwroot' 
            //DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            //defaultFilesOptions.DefaultFileNames.Clear();
            //defaultFilesOptions.DefaultFileNames.Add("sample.html");
            //app.UseFileServer();

            //FileServerOptions fileServerOptions = new FileServerOptions();  //this COMBINES the functionality of UseStaticFiles, UseDefaultFiles and UseDirectoryBrowser middleware (making the use of "DefaultFilesOptions" unnecessary)
            //fileServerOptions.DefaultFilesOptions.DefaultFileNames.Clear();
            //fileServerOptions.DefaultFilesOptions.DefaultFileNames.Add("sample.html");
            //app.UseFileServer(fileServerOptions);

            app.UseStaticFiles();//no longer using this as it is replaced by the 'Views' folder and its information (copy/pasted the content into the Views folder)

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            ////EXAMPLE of using  middleware. //use the Use method to ensure the rest of the middleware is called
            //app.Use(async (context, next) =>
            //{
            //    await context.Response.WriteAsync("Hello from 1st Middleware !");
            //    await next();
            //});

            //app.Run(async (context) => {    //EXAMPLE of Terminal Middleware (nothing below this method would be called if anything existed below)
            //    await context.Response.WriteAsync("Hello from 2st Middleware !");
            //});
        }
    }
}
