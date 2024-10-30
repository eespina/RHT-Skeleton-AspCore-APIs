using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserApi.Services.Interfaces;
using UserApi.ViewModels;

namespace UserApi.Controllers
{
    public class UserController : Controller
    {
        IUserService userService;
        //IMailService mailService;//TODO - IF we need this, and I mean "IF", we'll create a Mail/Notification API and move logic there; keeping this reference if the logic applies here
        IAuthenticateUserService authenticateService;
        private readonly ILogger<UserController> logger;

        public UserController(IUserService UserService, ILogger<UserController> logger, IAuthenticateUserService authenticateService)
        {
            this.userService = UserService;
            this.logger = logger;
            //this.mailService = mailService;
            this.authenticateService = authenticateService;
        }

        [HttpGet]
        public async Task<IEnumerable<OwnerViewModel>> Get()//public IEnumerable<string> Get()
        {
            //var returnList = new string[] { "User1", "User2" };
            var returnList = await userService.FindUsers();
            return returnList;
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> Get(string username)
        {
            var user = await userService.FindUserByUserName(username);
            return Ok(user);
        }

        [Route("registeruser"), HttpPost]
        public async Task<IActionResult> Post([FromBody] OwnerViewModel model)
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

        ///// <summary>
        ///// This currently does nothing, it just returns mocked data back to the client-side
        ///// </summary>
        ///// <param name="ownerViewModel">material from the client-side application to save over the existing datat for a particular client</param>
        ///// <returns>SHOULD return something, we're currently mocking </returns>
        //[HttpPut("{id}"), Authorize]
        //public async Task<IActionResult> Put([FromBody] OwnerViewModel ownerViewModel)
        //{
        //    //TODO - this needs to be moved into a service area in this project and NOT stay in this Controller
        //    var areCredentialsSet = false;
        //    if (ownerViewModel.IsChangingCredentials)
        //    {
        //        Request.Headers.TryGetValue("old-password", out var encryptedOldPassword);
        //        Request.Headers.TryGetValue("new-password", out var encryptedNewPassword);
        //        if (!string.IsNullOrWhiteSpace(encryptedOldPassword) && !string.IsNullOrWhiteSpace(encryptedNewPassword))
        //        {
        //            var decryptedOldPassword = await authenticateService.DecryptStringAES(encryptedOldPassword);
        //            var decryptedNewPassword = await authenticateService.DecryptStringAES(encryptedNewPassword);

        //            if (await authenticateService.EnsureAdministeringUserIsValid(ownerViewModel.UserId.ToString(), decryptedOldPassword))
        //            {
        //                var newPw = await authenticateService.ChangeCredentialsAsync(ownerViewModel.UserId.ToString(), decryptedNewPassword);
        //                if (!string.IsNullOrWhiteSpace(newPw))
        //                {
        //                    areCredentialsSet = true;
        //                }
        //            }
        //        }

        //        if (!areCredentialsSet)
        //        {
        //            return Unauthorized("Password Not Changed. User Not Updated");
        //        }
        //    }

        //    //do any other updating here
        //    var updatedUser = await userService.UpdateUser(ownerViewModel);

        //    //return BadRequest("There is no use-able password information present in the Header");
        //    return Ok(updatedUser);
        //}

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(string userId)
        {
            var isDeletionSuccessful = await userService.DeleteUser(userId);
            return Ok(isDeletionSuccessful);
        }
    }
}
