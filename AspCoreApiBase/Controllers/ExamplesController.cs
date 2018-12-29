using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspCoreBase.Services.Interfaces;
using Microsoft.Extensions.Logging;
using AspCoreBase.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace AspCoreApiBase.Controllers
{
    [Route("api/[controller]")]
    //[ApiController]
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

        //[HttpGet]
        //[ProducesResponseType(404)]
        //public async Task<IEnumerable<UserViewModel>> Index()
        //{
        //    var users = await userService.FindUsers();
        //    return users;
        //}

        // GET: api/values
        [HttpGet, Authorize]
        //public ActionResult<IEnumerable<UserViewModel>> Get()//public IEnumerable<string> Get()
        public async Task<IEnumerable<UserViewModel>> Get()//public IEnumerable<string> Get()
        {
            //var returnList = new string[] { "Example1", "Example2" };
            var returnList = await userService.FindUsers();

            return returnList;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
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
