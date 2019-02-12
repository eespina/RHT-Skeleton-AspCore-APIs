using AspCoreBase.Services.Interfaces;
using AspCoreBase.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            TokenHandleViewModel tokenHandler;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                tokenHandler = await authenticateService.CreateToken(model);
                if (tokenHandler != null)
                {
                    return Ok(tokenHandler);
                }

                return Unauthorized();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout(LoginViewModel model)
        {
            await authenticateService.SignOutAsync();

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] LoginViewModel model)
        {
            TokenHandleViewModel tokenHandler;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var currentLoggedInUser = HttpContext.User;
                if (currentLoggedInUser != null)
                {
                    //var userMapped = convertLoginUserInto

                    //var newlyCreatedUser = userService.CreateNewUser(model, currentLoggedInUser);
                    //if (newlyCreatedUser != null)
                    //{
                    //    tokenHandler = await authenticateService.CreateToken(model);
                    //    if (tokenHandler != null)
                    //    {
                    //        return Ok(tokenHandler);
                    //    }
                    //}
                }

                return Unauthorized();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}