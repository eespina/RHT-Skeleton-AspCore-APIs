using AspCoreBase.Services.Interfaces;
using AspCoreBase.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AspCoreApiBase.Controllers
{
    [Route("api/[controller]")]
    public class RegisterOwnerController : Controller
    {
        private readonly IAuthenticateService authenticateService;

        public RegisterOwnerController(IAuthenticateService authenticateService)
        {
            this.authenticateService = authenticateService;
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> Post([FromBody] OwnerViewModel model)
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
                    OwnerViewModel newlyCreatedUser = null;// userService.CreateNewUser(model, currentLoggedInUser);
                    if (newlyCreatedUser != null)
                    {
                        tokenHandler = await authenticateService.CreateToken(new LoginViewModel { Password = model.Password, Username = model.UserName });
                        if (tokenHandler != null)
                        {
                            return Ok(tokenHandler);
                        }
                    }
                }

                return Unauthorized();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        //// GET: api/values
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
