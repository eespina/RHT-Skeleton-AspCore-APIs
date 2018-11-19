using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace AspCoreBase
{
	public class Program
	{
		/*
         *  still just a console app in terms of having this 'Main' method that just listens for web requests
         *  when newly created as an Empty project
         *  */
		public static void Main(string[] args)
		{
			//builds a web hosts and listens for requests
			//only method that is included in the Empty web project
			BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)  //creates a default builder for our web hosts - also sets up a default configuration file that can be readily available
				.ConfigureAppConfiguration(SetupConfiguration)
				.UseStartup<Startup>()  //tells it what class to use ( in this case, Startup.cs) to set up HOW to listen for web requests
				.Build();   //Then it just builds it so it can then runn it

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
