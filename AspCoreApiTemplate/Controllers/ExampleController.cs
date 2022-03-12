using AspCoreApiTemplate.Services.Interfaces;
using AspCoreApiTemplate.ViewModels;
using Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace AspCoreApiTemplate.Controllers
{
    /// <summary>
    /// This SHOULD be moved into it's own MicroService API including everything alongside it.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ExampleController : Controller
    {
        IExampleService exampleService;
        IMailService mailService;
        IErrorHandler errorHandler;
        private readonly ILogger<UserController> logger;

        public ExampleController(ILogger<UserController> logger, IExampleService exampleService, IMailService mailService, IErrorHandler _errorHandler)
        {
            this.logger = logger;
            this.exampleService = exampleService;
            this.mailService = mailService;
            errorHandler = _errorHandler;
        }

        [HttpGet, Authorize]
        public async Task<IEnumerable<ExampleViewModel>> Get()
        {
            //var returnList = new string[] { "Example1", "Example2" };
            var returnList = await exampleService.GetExamples();
            return returnList;
        }

        //[HttpGet("{exampleId}"), Authorize]
        //public async Task<IActionResult> Get(string exampleId)
        //{
        //    var example = await exampleService.GetExample(exampleId);
        //    return Ok(example);
        //}

        [HttpPost, Authorize]
        public async Task<IActionResult> Post([FromBody] ExampleViewModel exampleViewModel)
        {
            //var isCreationSuccessful = await exampleService.CreateExample(exampleViewModel);
            //return Ok(isCreationSuccessful);

            var example = new ExampleViewModel();
            logger.LogTrace($"inside {MethodBase.GetCurrentMethod().Name}.");

            if (exampleViewModel == null || !ModelState.IsValid)
            {
                return UnprocessableEntity(example.Error = new ErrorViewModel
                {
                    ErrorMessage = await errorHandler.GetErrorMessage(ModelState)
                });
            }
            else
            {
                example = await exampleService.CreateExample(exampleViewModel);
                return Ok(example);
            }
        }

        //[HttpPut, Authorize]
        //public async Task<IActionResult> Put([FromBody] ExampleViewModel exampleViewModel)
        //{
        //    var returnExampleViewModel = await exampleService.UpdateExample(exampleViewModel);
        //    return Ok(returnExampleViewModel);
        //}

        [HttpDelete("{exampleId}"), Authorize]
        public async Task<IActionResult> Delete(string exampleId)
        {
            var isDeletionSuccessful = await exampleService.DeleteExample(exampleId);
            return Ok(isDeletionSuccessful);
        }
    }
}
