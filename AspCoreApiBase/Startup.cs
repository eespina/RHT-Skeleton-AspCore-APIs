using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspCoreApiBase
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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
