using AspCoreBase.Data;
using AspCoreBase.Data.Entities.Authority;
using AspCoreBase.Services;
using AspCoreBase.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AspCoreApiBase
{
    public class Startup
    {
        private readonly IConfiguration config;
        private readonly IHostingEnvironment environment;

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            config = configuration;
            this.environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            /*  Examples of Service implementation  */
            //services.AddTransient<IMailService, MailService>();   //light-weight and used only when needed
            //services.AddScoped<IMailService, MailService>();  //heavier with the service being elongated for a longer period
            //services.AddSingleton<IMailService, MailService>();   // heaviest as it will be sustained in the application for the entirety of the application running

            //services.AddCors();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            //Configure Identity. IdentityRole = in case we want to configure roles, its a type of data that can store info about a user,
            //but we can derive IdentityRole to have our own role class (but leaving it as IdentityRole for now)
            services.AddIdentity<AuthorityUser, IdentityRole>(cfg =>
            {   //this part of the configuration allows us to make decision about different parts of the system
                cfg.User.RequireUniqueEmail = true;//avoids duplicate emails since it should be a unique identification material anyway
                cfg.Password.RequireDigit = true;
                cfg.Password.RequireUppercase = true;
                cfg.Password.RequireLowercase = true;

            }).AddEntityFrameworkStores<AuthorityDbContext>();   //add this to specify where the data shuld be coming from. We want to explicitly say what type of context to use internally in Identity when it wants to get at objects stored in the Database.

            //by default, when you add Identity, you are supporting authentication based on cookies. Here we are now defining two kinds of authenitcation that we're going to support
            // once implemented,
            services.AddAuthentication()
                .AddCookie()
                .AddJwtBearer(cfg =>
                {//we need to tell this method about the bearer token that is created in 'CreateToken' method of Account Controller
                 //need to set up the token validation parameters
                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = config["Tokens:Issuer"],
                        ValidAudience = config["Tokens:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Tokens:Key"]))
                    };
                });

            ////-ANOTHER version found on the internet that will NOT be used
            ////Firstly, we register the JWT authentication middleware by calling the method AddAuthentication on the ISerivceCollectioninterface.
            ////Next, we specify the authentication scheme JwtBearerDefaults.AuthenticationScheme. We also provide some parameters that will be used while validating JWT
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //.AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,    //The issuer is the actual server that created the token
            //        ValidateAudience = true,  //The receiver of the token is a valid recipient
            //        ValidateLifetime = true,  //The token has not expired 
            //        ValidateIssuerSigningKey = true,  //The signing key is valid and is trusted by the server 

            //        ValidIssuer = "http://localhost:5000",
            //        ValidAudience = "http://localhost:5000",
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"))
            //    };
            //});

            //creating and configuring the database provider using this AspCoreBaseDbContext
            services.AddDbContext<VillageDbContext>(cfg =>
            {
                cfg.UseSqlServer(this.config.GetConnectionString("aspCoreBaseConnectionString"));
            });

            services.AddDbContext<AuthorityDbContext>(cfg =>
            {
                cfg.UseSqlServer(this.config.GetConnectionString("aspCoreBaseAuthorityConnectionString"));
            });

            services.AddAutoMapper();

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

            // adds all the services that the subsystem requires to run ASP.NET MVC and would get rid of an 'IServiceCollection.AddMvc' error
            services.AddMvc(opt =>
            {
                if (environment.IsProduction())
                {
                    opt.Filters.Add(new RequireHttpsAttribute());  //  it can also be used inside Controllers or Actions to require https on only certain controllers or actions. This applies it to EVERYWHERE on the site
                }
            }).AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //in 'Properties --> Debug' there is an area that sets up which environment the project is set as (Environment Variables) and is used every time the debug build is run
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();    //when Exceptions (not bad status codes responses) occur, use this - only for DEVELOPERS use only
                app.UseExceptionHandler("/discrepancy");  //for non-developement environments, will show a friendlier error page
            }
            else
            {
                app.UseExceptionHandler("/discrepancy");  //for non-developement environments, will show a friendlier error page
            }

            //// Shows UseCors with CorsPolicyBuilder.
            //app.UseCors(builder =>
            //   builder.WithOrigins("http://localhost:62761"));
            app.UseCors("CorsPolicy");

            //When the web server comes up, we're going to tell it to add the service of 'serving static files' as something that is allowed to do
            //default behaviour is to ONLY serve files from the 'wwwroot' directory where, for security reasons
            //  (among others), wwwroot is treated as the root of the web server created (hence the url structure)
            // you can pass a lambda expression so that they may configure themselves
            app.UseStaticFiles();//no longer using this as it is replaced by the 'Views' folder and its information (copy/pasted the content into the Views folder)

            //this tells it that we want to use the Authentication and use any of the configuration that was done inside ConfigureServices
            //should never need more than this one line inside the configure method (here) to turn on Authentication
            //make sure this is before 'UseMvc()' method because Configure method is the pipeline and order of precendce matters here.
            //placing this AFTER, will specify that the MVC method will not know about Authetication;
            app.UseAuthentication();

            // tells the compile that we want to listern for request, then attempt to map them to a Controller (which then maps to a View)
            // you can pass a lambda expression (as done here) so that they may configure themselves
            //      here, the lambda in parameters is setting up the 'Route' configuration
            app.UseMvc(config =>
            {
                //what's happening here is as follows: when the request sees a URL come in after the server name and port name,
                //      map that "/users/manage" to the Controller and directly to the Action specifically
                //This 'Default' paramerter is how we set the hehaviour if there is no URL after the domain (if the conroller and action part of the url are missing)
                //      This is the DEFAULT behavior setting(s)
                config.MapRoute("Default",
                    "{controller}/{action}/{id?}", //the '?' means that the part of the URL is OPTIONAL
                    //May want to switch DEFAULTs to some controller that tests everything on start up and then notifies someone/something when it is erroneous
                    new { controller = "Values", Action = "Get" });
            });

            #region OUT OF THE BOX MATERIAL WITH AN EMPTY WEB PROJECT
            ////if i am not going to be serving html files directly, i would not really need this method,
            ////if i have an index.html file in the root of my wwwroot directory, it will use this as the root
            ////  so now, i only need the localhost:[port#] in the URL and no name or file extension
            //// you can pass a lambda expression so that they may configure themselves
            //app.UseDefaultFiles();

            ////included on an empty project
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            ////included on an empty project
            ////this does not look at the request at all, it just writes out a string of the content. 
            ////    if there are HTML elements, THEN it will render those as well. Otherwise it is just text
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("<h1>Hello World! Again!</h1>");
            //});
            #endregion

            #region OUT OF THE BOX MATERIAL WITH AN .NET CORE WEBAPI TEMPLATE
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}

            //app.UseHttpsRedirection();
            //app.UseMvc();
            #endregion
        }
    }
}
