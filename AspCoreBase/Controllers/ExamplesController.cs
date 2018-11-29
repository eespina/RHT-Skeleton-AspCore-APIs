using AspCoreBase.Services.Interfaces;
using AspCoreBase.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspCoreBase.Controllers
{
	//[Produces("application/json")]
	[Route("api/[controller]")]
	//[ApiController]
	public class ExamplesController : Controller
	{
		IUserService userService;
		IPropertyService propertyService;
		IMailService mailService;
		private readonly ILogger<ExamplesController> logger;

		public ExamplesController(IUserService UserService, ILogger<ExamplesController> logger, IPropertyService propertyService, IMailService mailService)
		{
			this.userService = UserService;
			this.logger = logger;
			this.propertyService = propertyService;
			this.mailService = mailService;
		}

		[HttpGet]
		[ProducesResponseType(404)]
		public async Task<IEnumerable<UserViewModel>> Index()
		{
			var users = await userService.FindUsers();
			return users;
		}

		//public IEnumerable<Example> Get()
		//{
		//	using (var exampleEntities = new ExampleDbEntities())
		//	{
		//		return exampleEntities.Examples.ToList();
		//	}
		//}

		//public Example Get(string par)
		//{
		//	using (var exampleEntities = new ExampleDbEntities())
		//	{
		//		if (Int32.TryParse(par, out int n))
		//		{
		//			return exampleEntities.Examples.FirstOrDefault(e => e.exampleId == n);
		//		}
		//		return exampleEntities.Examples.FirstOrDefault(e => e.exampleString == par);
		//	}
		//}
	}
}
