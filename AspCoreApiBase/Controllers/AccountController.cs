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

        public AccountController(ILogger<AccountController> logger, IAuthenticateService authenticateService, IConfiguration config)
        {
            this.logger = logger;
            this.config = config;
            this.authenticateService = authenticateService;
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
    }
}