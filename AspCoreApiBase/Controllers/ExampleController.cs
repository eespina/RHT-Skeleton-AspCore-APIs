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
    public class ExampleController : Controller
    {
        IExampleService exampleService;
        IMailService mailService;
        private readonly ILogger<UserController> logger;

        public ExampleController(ILogger<UserController> logger, IExampleService exampleService, IMailService mailService)
        {
            this.logger = logger;
            this.exampleService = exampleService;
            this.mailService = mailService;
        }

        [HttpGet, Authorize]
        public async Task<IEnumerable<ExampleViewModel>> Get()
        {
            //var returnList = new string[] { "Example1", "Example2" };
            var returnList = await exampleService.GetExamples();
            return returnList;
        }

        [HttpGet("{exampleId}"), Authorize]
        public async Task<IActionResult> Get(string exampleId)
        {
            var example = await exampleService.GetExample(exampleId);
            return Ok(example);
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> Post([FromBody] ExampleViewModel exampleViewModel)
        {
            var isCreationSuccessful = await exampleService.CreateExample(exampleViewModel);

            return Ok(isCreationSuccessful);
        }

        /// <summary>
        /// This currently does nothing, it just returns mocked data back to the client-side
        /// </summary>
        /// <param name="ownerViewModel">material from the client-side application to save over the existing datat for a particular client</param>
        /// <returns>SHOULD return something, we're currently mocking </returns>
        [HttpPut, Authorize]
        public async Task<IActionResult> Put([FromBody] ExampleViewModel exampleViewModel)
        {
            var isUpdateSuccessful = await exampleService.UpdateExample(exampleViewModel);
            return Ok(isUpdateSuccessful);
        }

        [HttpDelete("{exampleId}"), Authorize]
        public async Task<IActionResult> Delete(string exampleId)
        {
            var isDeletionSuccessful = await exampleService.DeleteExample(exampleId);
            return Ok(isDeletionSuccessful);
        }
    }
}
