using AspCoreBase.Services.Interfaces;
using AspCoreBase.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspCoreApiBase.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExamplesController : Controller
    {
        IUserService userService;
        IPropertyService propertyService;
        IMailService mailService;
        private readonly ILogger<ExamplesController> logger;

        public ExamplesController(IUserService UserService, ILogger<ExamplesController> logger, IPropertyService propertyService, IMailService mailService)
        {
            this.userService = UserService;
            this.logger = logger;
            this.propertyService = propertyService;
            this.mailService = mailService;
        }

        [HttpGet, Authorize]
        public async Task<IEnumerable<OwnerViewModel>> Get()//public IEnumerable<string> Get()
        {
            //var returnList = new string[] { "Example1", "Example2" };
            var returnList = await userService.FindUsers();
            return returnList;
        }

        // GET api/values/[username]
        [HttpGet("{username}")]
        public async Task<OwnerViewModel> Get(string username)
        {
            var user = await userService.FindUser(username);
            return user;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
