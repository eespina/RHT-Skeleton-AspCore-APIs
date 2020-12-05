using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspCoreBase
{
    public class StartupStaging : Startup
    {
        public StartupStaging(IConfiguration config) : base(config) { }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            base.Configure(app, env);
        }
    }
}