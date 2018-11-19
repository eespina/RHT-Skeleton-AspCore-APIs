using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AspCoreBase.Services;
using AspCoreBase.Services.Interfaces;
using AspCoreBase.ViewModels;

namespace AspCoreBase.Controllers
{
	[Authorize]
	public class SettingsController : Controller
	{
		private readonly ILogger<SettingsController> logger;
		private readonly IAuthenticateService authenticateService;

		public SettingsController(ILogger<SettingsController> logger, IAuthenticateService authenticateService)
		{
			this.logger = logger;
			this.authenticateService = authenticateService;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Help()
		{
			return View();
		}

		public IActionResult ProfileManagement()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ProfileManagement(LoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					if (await authenticateService.ChangeCredentialsAsync(model))
					{
						return RedirectToAction("ProfileManagement", "Settings");
					}
				}
				catch (System.Exception ex)
				{
					logger.LogWarning("ERROR inside SettingsController.ProfileManagement.HttpPost - " + ex);
				}
			}
			else
			{
				logger.LogWarning("WARNING inside SettingsController.ProfileManagement.HttpPost - Authentication model state was Invalid");
			}

			ModelState.AddModelError("", "Authentication credentials are Incorrect, Password NOT Changed!");

			return View();
		}
	}
}
