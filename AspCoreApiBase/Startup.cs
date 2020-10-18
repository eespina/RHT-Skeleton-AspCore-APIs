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
