using AspCoreBase.Services;
using AspCoreBase.Services.Interfaces;
using AspCoreBase.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AspCoreApiBase.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
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
        [Route("login"), HttpPost]
        public async Task<IActionResult> Login()
        {
            OwnerViewModel loggedInUser;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                Request.Headers.TryGetValue("username", out var encryptedUserName);
                Request.Headers.TryGetValue("password", out var encryptedPassword);
                if (!string.IsNullOrWhiteSpace(encryptedUserName) && !string.IsNullOrWhiteSpace(encryptedPassword))
                {
                    var decryptedUserName = await authenticateService.DecryptStringAES(encryptedUserName);
                    var deryptedPassword = await authenticateService.DecryptStringAES(encryptedPassword);
                    loggedInUser = await authenticateService.CreateToken(decryptedUserName, deryptedPassword);
                    if (loggedInUser != null)
                    {
                        return Ok(loggedInUser);
                    }
                }

                return Unauthorized();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Authorize]
        [Route("logout"), HttpPost]
        public async Task<IActionResult> Logout()
        {
            await authenticateService.SignOutAsync();

            return NoContent();
        }

        /// <summary>
        /// This has not yet been tested thoroughly so I hope it works
        /// </summary>
        /// <param name="model">User model from teh client-side</param>
        /// <returns>Newly created OwnderViewModel to the client</returns>
        [Authorize]
        [Route("registeruser"), HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] OwnerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var requestHeadersCount = Request.Headers.Count;
                var requestHeadersPw = Request.Headers["reticulatingsplines"];
                Request.Headers.TryGetValue("reticulatingsplines", out var encryptedPassword);
                if (!string.IsNullOrWhiteSpace(encryptedPassword))
                {
                    var decryptedPassword = await authenticateService.DecryptStringAES(encryptedPassword); //TODO - THINK about just not keeping an encrypted key in the database instead of going through the process.
                    OwnerViewModel newlyCreatedUser = await userService.CreateNewUser(model, decryptedPassword);
                    if (newlyCreatedUser != null)
                    {
                        return Ok(newlyCreatedUser);
                    }
                }

                return NotFound("USER NOT CREATED");

                //return Ok(new OwnerViewModel {LastName = "LastNameTest", FirstName = "FirstNameTest", UserName = "UserNameTest" });   //just used for Testing where i do NOT want to 
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}