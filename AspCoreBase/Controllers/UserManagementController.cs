using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspCoreBase.Services.Interfaces;
using AspCoreBase.ViewModels;

namespace AspCoreBase.Controllers
{
	[Authorize]
	public class UserManagementController : Controller
	{
		IUserService userService;
		IPropertyService propertyService;
		IMailService mailService;
		private readonly ILogger<UserManagementController> logger;

		public UserManagementController(IUserService UserService, ILogger<UserManagementController> logger, IPropertyService propertyService, IMailService mailService)
		{
			this.userService = UserService;
			this.logger = logger;
			this.propertyService = propertyService;
			this.mailService = mailService;
		}

		public async Task<IActionResult> Index()
		{
			ViewBag.TitleText = "User Users - Index";
			var users = await userService.FindUsers();
			return users == null ? View() : View(users);
		}

		[HttpGet("NewUser")]
		public async Task<IActionResult> CreateUser()
		{
			ViewBag.NewUserTitleText = "New User Creation Page";
			var propertyList = await propertyService.GetProperties();

			var properties = propertyList.ToList();
			properties.Insert(0, new PropertyViewModel { BuildingName = "Select Building" });
			ViewBag.PropertyList = properties;

			var userTypes = new List<UserType> {	// Add 'Master' later (if need be)
				new UserType { Id = 0, Name = "Select User Type" }
				, new UserType { Id = 1, Name = "Admin" }
				, new UserType { Id = 2, Name = "Owner" }
				, new UserType { Id = 3, Name = "Renter" }
				, new UserType { Id = 4, Name = "Doorman" }
				, new UserType { Id = 5, Name = "Maintenance" }
				, new UserType { Id = 6, Name = "Janitor" }
				, new UserType { Id = 7, Name = "Vendor" }
				, new UserType { Id = 8, Name = "Unassigned" }
			};

			ViewBag.UserTypeList = userTypes;
			return View();
		}

		[HttpPost("NewUser")]
		public async Task<IActionResult> CreateUser(UserViewModel userViewModel)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var newUser = await userService.CreateNewUser(userViewModel, this.User);

					if (newUser != null)
					{
						//Send Email Registration Invitiation
						try
						{
							if (!await mailService.SaveEmailComposition(newUser, this.User))
							{
								logger.LogWarning("Warning Email did NOT get saved into it's Staging system");
							}

							//var emailComposition = await mailService.SaveEmailComposition(newUser);//TODO - see if this should be a part of the SendMessgae instead of being separated

							//await mailService.SendMessage(emailComposition);
						}
						catch (Exception ex)
						{
							logger.LogError("ERROR Email - " + ex);	//Just log, nothing needs to happen. We just need to be notified
						}

						//TODO - save the chosen Property and the UserID to the UserProperty Table (depending upon the UserType) - I dont beleive there is reason to have this in the DB
						if (await propertyService.CreatePropertyUserConnection(newUser))
						{
							ViewBag.PostMessage = userViewModel.FirstName + " has been Created! An email has been sent to " + userViewModel.Email + " and the Registration Process has begun with a time limit of 42 hours";  //wait where is this? and how does it show post submit and not before?

							ModelState.Clear();
							ViewBag.PostMessage = "Creating User, Shouldn't be long now...";
							newUser.ChosenCredentials = string.Empty;

							//redirect to the User Detail Page of the NEW user using the 'UserViewModel'
							return View("GetUserDetail", newUser);
						}

						ViewBag.PostMessage = "User was NOT Created! A Property Connection could NOT be created. Please Contact your administrator at your earliest convenience";
					}

					ViewBag.PostMessage = "User was NOT Created! Please Contact your administrator at your next convenience";
				}
				catch (Exception ex)
				{
					logger.LogError("ERROR inside UserService.CreateVillageUser - " + ex);
					ViewBag.PostMessage = "Creating User, Shouldn't be long now...";
				}
			}
			else
			{
				var errorMessages = string.Empty;
				foreach (var em in ModelState.Values.Where(v => v.ValidationState == ModelValidationState.Invalid))
				{
					var specificErrorMessage = "For " + em.AttemptedValue + " , ";
					foreach (var sem in em.Errors)
					{
						specificErrorMessage += sem.ErrorMessage;
					}
					errorMessages += specificErrorMessage;
					errorMessages += "<br />";
				}
				ViewBag.PostMessage = "The Following are invalid - " + errorMessages;
			}

			return View("CreateUser", ViewBag.PostMessage);	//TODO - ERRONEOUS at the moment - FIX THIS, but how ??? better yet, DO SOME CLIENT SIDE VALIDATION INSTEAD
		}

		[HttpGet("UserDetail")]
		public IActionResult GetUserDetail(UserViewModel userViewModel)
		{
			ViewBag.TitleText = "User Detail's Page";
			return View(userViewModel);
		}
	}
}
