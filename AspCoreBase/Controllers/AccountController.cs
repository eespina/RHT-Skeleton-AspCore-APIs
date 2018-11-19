using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using AspCoreBase.Services.Interfaces;
using AspCoreBase.ViewModels;

namespace AspCoreBase.Controllers
{
	public class AccountController : Controller
	{
		private readonly ILogger<AccountController> logger;
		private readonly IAuthenticateService authenticateService;
		private readonly IConfiguration config;

		public AccountController(ILogger<AccountController> logger, IAuthenticateService authenticateService, IConfiguration config)
		{
			this.logger = logger;
			this.config = config;
			this.authenticateService = authenticateService;
		}

		public IActionResult Login()
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");  // this is a return to a specific Controller and Action
			}

			ViewBag.TitleText = "ViewBag - Login";

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				var result = await authenticateService.PasswordSign(model);
				if (result != null)
				{
					if (result.Succeeded)
					{
						if (Request.Query.Keys.Contains("ReturnUrl"))
						{
							return Redirect(Request.Query["ReturnUrl"].First());
						}
						else
						{
							return RedirectToAction("Index", "Home");
						}
					}
				}
			}

			//  If we get all the way down here, it is because there is an error with the ENTIRE model.
			ModelState.AddModelError("", "Failed to Login");

			return View();
		}

		[HttpGet]
		public async Task<IActionResult> Logout(LoginViewModel model)
		{
			await authenticateService.SignOutAsync();

			return RedirectToAction("Login", "Account");
		}

		[HttpPost]  // make it post because we do NOT want anything in the header or in the query string that is confidential
		public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				var tokenHandler = await authenticateService.CreateToken(model);
				return Created("", tokenHandler);
			}

			return BadRequest();
		}
	}
}
