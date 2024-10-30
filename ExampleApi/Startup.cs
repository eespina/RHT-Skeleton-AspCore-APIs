using AutoMapper;
using ExampleApi.Data;
//using ExampleApi.Models;//there's' no more appsettings since i dont think we need them. there's an identityauthrority think ing th appsettings, but i dont think thats needed
using ExampleApi.Services;
using ExampleApi.Services.Interfaces;
using Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace ExampleApi
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

            services.AddControllers();
            services.AddAutoMapper(typeof(Startup));
            services.AddCors(options => { options.AddPolicy("CorsPolicy", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()); });

            services.AddDbContext<ExampleDbContext>(cfg =>
            {
                cfg.UseSqlServer(this.Configuration.GetConnectionString("ExampleConnectionString"));
            });

            #region SERVICES
            //services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddScoped<IExampleDbRepository, ExampleDbRepository>();
            services.AddScoped<IErrorHandler, ErrorHandler>();
            services.AddTransient<IExampleService, ExampleService>();
            #endregion

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ExampleApi", Version = "v1", Description = "Asp .NET Core API's Example API back end processing API" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ExampleApi v1"));
            }
            app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();
            app.UseRouting();
            //app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
