using AspCoreApiTemplate.Services.Interfaces;
using AspCoreApiTemplate.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspCoreApiTemplate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        IUserService userService;
        IMailService mailService;
        IAuthenticateService authenticateService;
        private readonly ILogger<UserController> logger;

        public UserController(IUserService UserService, ILogger<UserController> logger, IMailService mailService, IAuthenticateService authenticateService)
        {
            this.userService = UserService;
            this.logger = logger;
            this.mailService = mailService;
            this.authenticateService = authenticateService;
        }

        [HttpGet, Authorize]
        public async Task<IEnumerable<OwnerViewModel>> Get()//public IEnumerable<string> Get()
        {
            //var returnList = new string[] { "User1", "User2" };
            var returnList = await userService.FindUsers();
            return returnList;
        }

        [HttpGet("{username}"), Authorize]
        public async Task<IActionResult> Get(string username)
        {
            var user = await userService.FindUserByUserName(username);
            return Ok(user);
        }

        [HttpPost, Authorize]
        public void Post([FromBody] string value)
        {
        }

        /// <summary>
        /// This currently does nothing, it just returns mocked data back to the client-side
        /// </summary>
        /// <param name="ownerViewModel">material from the client-side application to save over the existing datat for a particular client</param>
        /// <returns>SHOULD return something, we're currently mocking </returns>
        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> Put([FromBody] OwnerViewModel ownerViewModel)
        {
            var areCredentialsSet = false;
            if (ownerViewModel.IsChangingCredentials)
            {
                Request.Headers.TryGetValue("old-password", out var encryptedOldPassword);
                Request.Headers.TryGetValue("new-password", out var encryptedNewPassword);
                if (!string.IsNullOrWhiteSpace(encryptedOldPassword) && !string.IsNullOrWhiteSpace(encryptedNewPassword))
                {
                    var decryptedOldPassword = await authenticateService.DecryptStringAES(encryptedOldPassword);
                    var decryptedNewPassword = await authenticateService.DecryptStringAES(encryptedNewPassword);

                    if (await authenticateService.EnsureAdministeringUserIsValid(ownerViewModel.UserId.ToString(), decryptedOldPassword))
                    {
                        var newPw = await authenticateService.ChangeCredentialsAsync(ownerViewModel.UserId.ToString(), decryptedNewPassword);
                        if (!string.IsNullOrWhiteSpace(newPw))
                        {
                            areCredentialsSet = true;
                        }
                    }
                }

                if (!areCredentialsSet)
                {
                    return Unauthorized("Password Not Changed. User Not Updated");
                }
            }

            //do any other updating here
            var updatedUser = await userService.UpdateUser(ownerViewModel);

            //return BadRequest("There is no use-able password information present in the Header");
            return Ok(updatedUser);
        }

        [HttpDelete("{userId}"), Authorize]
        public async Task<IActionResult> Delete(string userId)
        {
            var isDeletionSuccessful = await userService.DeleteUser(userId);
            return Ok(isDeletionSuccessful);
        }
    }
}
