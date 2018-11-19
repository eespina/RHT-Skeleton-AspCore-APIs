using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspCoreBase.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		//use Existing Controllers that have a view for reference on how to Create this page

		//The HomeViewModel should probably contain a handfull of other viewmodels

		//Add a View and return a HomeViewModel (with a lot of other ViewModels inside it) to the View so that it uses Razor to set the page

		//Place an [Authorize] tab over the methods (See if using a Class Level [Authorize] tabs is a better idea)

		//Switch the Routing to go to the Login Page and then redirect to the HomeControllers, 'Index' method View when succesfull




		public HomeController() { }

		[Route("Home")]
		public IActionResult Index()
		{
			ViewBag.TitleText = "Home - Index";
			return View();
		}
	}
}
