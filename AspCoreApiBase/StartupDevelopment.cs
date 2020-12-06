using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspCoreBase
{
    //public class StartupDevelopment : Startup
    //{
    //    public StartupDevelopment(IConfiguration config) : base(config) { }

    //    public override void ConfigureServices(IServiceCollection services)
    //    {
    //        services
    //            .AddIdentityServer()
    //            .AddDeveloperSigningCredential()
    //            .AddInMemoryApiResources(IdentityProviderSeedData.GetApiResourceses())
    //            .AddInMemoryApiScopes(IdentityProviderSeedData.GetApiScopes())
    //            .AddInMemoryClients(IdentityProviderSeedData.Clients);

    //        base.ConfigureServices(services);
    //    }

    //    public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    //    {
    //        app.UseDeveloperExceptionPage();
    //        app.UseIdentityServer();
    //        base.Configure(app, env);
    //    }
    //}
}