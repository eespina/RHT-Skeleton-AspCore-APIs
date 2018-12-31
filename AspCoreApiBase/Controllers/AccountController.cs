using AspCoreBase.Services.Interfaces;
using AspCoreBase.ViewModels;
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

                return NoContent();
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout(LoginViewModel model)
        {
            await authenticateService.SignOutAsync();

            return RedirectToAction("Login", "Account");
        }

        //[HttpPost]  // make it post because we do NOT want anything in the header or in the query string that is confidential
        //public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var tokenHandler = await authenticateService.CreateToken(model);
        //        return Created("", tokenHandler);
        //    }

        //    return BadRequest();
        //}

        //public IActionResult Login()
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Home");  // this is a return to a specific Controller and Action
        //    }

        //    ViewBag.TitleText = "ViewBag - Login";

        //    return View();
        //}

        //[Route("api/[controller]")]   -   DEFAULTly created code that webapi controller creates
        //public class AccountController : Controller
        //{
        //    // GET: api/values
        //    [HttpGet]
        //    public IEnumerable<string> Get()
        //    {
        //        return new string[] { "value1", "value2" };
        //    }

        //    // GET api/values/5
        //    [HttpGet("{id}")]
        //    public string Get(int id)
        //    {
        //        return "value";
        //    }

        //    // POST api/values
        //    [HttpPost]
        //    public void Post([FromBody]string value)
        //    {
        //    }

        //    // PUT api/values/5
        //    [HttpPut("{id}")]
        //    public void Put(int id, [FromBody]string value)
        //    {
        //    }

        //    // DELETE api/values/5
        //    [HttpDelete("{id}")]
        //    public void Delete(int id)
        //    {
        //    }
        //}
    }
}