using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using System;

namespace AspCoreApiTemplate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("Initializing main");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                logging.AddConsole();
                logging.AddDebug();
                logging.AddEventSourceLogger();
                logging.AddNLog();
            }).UseNLog()
            ;

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)  //creates a default builder for our web hosts - also sets up a default configuration file that can be readily available
                .ConfigureAppConfiguration(SetupConfiguration)
                .UseStartup<Startup>()  //tells it what class to use ( in this case, Startup.cs) to set up HOW to listen for web requests
                .Build();   //Then it just builds it so it can then run it

        private static void SetupConfiguration(WebHostBuilderContext ctx, IConfigurationBuilder builder)
        {
            builder.Sources.Clear();    // Remove the default configuration options
            var isFileOptional = false;
            var reloadOnPage = true; // this way, if we have a running application, it will reload the the file every time a change is made to it so that the program does NOT have to restart

            //load these files an then combine them into one store of central configuration so that they are mixed in to a single set of configuration that will be used.
            // Conflicts are handled by heirarchy (order of precendes being that the last added (chained) onto the method gets precedence)
            builder.AddJsonFile("remove-from-source-control-config.json", isFileOptional, reloadOnPage)  //tell the system that we require a file called 'whatevers in the parameter'
                                                                                                         //.AddXmlFile("config.xml", true)
                                                                                                         //.AddEnvironmentVariables()
                ;
        }
    }
}
