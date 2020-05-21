using AspCoreBase.Data.Entities.Authority;
using AspCoreBase.Services.Interfaces;
using AspCoreBase.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AspCoreApiBase.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> logger;
        private readonly IAuthenticateService authenticateService;
        private readonly IConfiguration config;
        private readonly IUserService userService;

        public AccountController(ILogger<AccountController> logger, IAuthenticateService authenticateService, IConfiguration config, IUserService userService)
        {
            this.logger = logger;
            this.config = config;
            this.authenticateService = authenticateService;
            this.userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            OwnerViewModel loggedInUser;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                loggedInUser = await authenticateService.CreateToken(model);
                if (loggedInUser != null)
                {
                    loggedInUser.Password = string.Empty;
                    return Ok(loggedInUser);
                }

                return Unauthorized();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> Logout([FromBody] LoginViewModel model)
        {
            await authenticateService.SignOutAsync();

            return NoContent();
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> RegisterUser([FromBody] OwnerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                OwnerViewModel newlyCreatedUser = await userService.CreateNewUser(model);
                if (newlyCreatedUser != null)
                {
                    return Ok(newlyCreatedUser);
                }
                else
                {
                    return NotFound("USER NOT CREATED");
                }

                //return Ok(new OwnerViewModel {LastName = "LastNameTest", FirstName = "FirstNameTest", UserName = "UserNameTest" });   //just used for Testing where i do NOT want to 
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}