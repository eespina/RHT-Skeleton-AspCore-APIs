using AspCoreBase.Services.Interfaces;
using AspCoreBase.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspCoreApiBase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamplesController : Controller
    {
        IUserService userService;
        IPropertyService propertyService;
        IMailService mailService;
        IAuthenticateService authenticateService;
        private readonly ILogger<ExamplesController> logger;

        public ExamplesController(IUserService UserService, ILogger<ExamplesController> logger, IPropertyService propertyService, IMailService mailService, IAuthenticateService authenticateService)
        {
            this.userService = UserService;
            this.logger = logger;
            this.propertyService = propertyService;
            this.mailService = mailService;
            this.authenticateService = authenticateService;
        }

        [HttpGet, Authorize]
        public async Task<IEnumerable<OwnerViewModel>> Get()//public IEnumerable<string> Get()
        {
            //var returnList = new string[] { "Example1", "Example2" };
            var returnList = await userService.FindUsers();
            return returnList;
        }

        [HttpGet("{username}"), Authorize]
        public async Task<IActionResult> Get(string username)  //inside the client app, we've mixed IUser and IExample (angular). Should NOT be an issue creating new POCO/POJO entities in the future
        {
            var user = await userService.FindUser(username);
            return Ok(user);
        }

        [HttpPost, Authorize]
        public void Post([FromBody]string value)
        {
        }

        /// <summary>
        /// This currently does nothing, it just returns mocked data back to the client-side
        /// </summary>
        /// <param name="ownerViewModel">material from the client-side application to save over the existing datat for a particular client</param>
        /// <returns>SHOULD return something, we're currently mocking </returns>
        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> Put([FromBody]  OwnerViewModel ownerViewModel)
        {
            Request.Headers.TryGetValue("old-password", out var encryptedOldPassword);
            Request.Headers.TryGetValue("new-password", out var encryptedNewPassword);
            if (!string.IsNullOrWhiteSpace(encryptedOldPassword) && !string.IsNullOrWhiteSpace(encryptedNewPassword))
            {
                var decryptedOldPassword = await authenticateService.DecryptStringAES(encryptedOldPassword);
                var decryptedNewPassword = await authenticateService.DecryptStringAES(encryptedNewPassword);

                //CHECK for the existing password to ensure they match, then UPDATE the existing user

                return Ok(new OwnerViewModel { UserName = decryptedNewPassword });
            }
            return BadRequest("There is no use-able password information present in the Header");
        }

        [HttpDelete("{id}"), Authorize]
        public void Delete(int id)
        {
        }
    }
}
