using ExampleApi.Services.Interfaces;
using ExampleApi.ViewModels;
using Extensions;
//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace ExampleApi.Controllers
{
    /// <summary>
    /// IDEALLY, this entire PROJECT would be in another solution, buuuuuuuuut may also be argued that it should all stay within the same
    /// soluion. I think it's just a matter of how lazy people get when debugging cross api's. Ideally, each api's modification would get
    /// deployed locally to the local machine's environment.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ExampleController : Controller
    {
        IExampleService exampleService;
        IErrorHandler errorHandler;
        private readonly ILogger<ExampleController> logger;

        public ExampleController(ILogger<ExampleController> logger, IExampleService exampleService, IErrorHandler _errorHandler)
        {
            this.logger = logger;
            this.exampleService = exampleService;
            errorHandler = _errorHandler;
        }

        [HttpGet]
        //[HttpGet, Authorize]
        public async Task<IEnumerable<ExampleViewModel>> Get()
        {
            var returnList = await exampleService.GetExamples();
            return returnList;
        }

        [HttpGet("{exampleId}")]
        //[HttpGet("{exampleId}"), Authorize]
        public async Task<IActionResult> Get(string exampleId)
        {
            var example = await exampleService.GetExample(exampleId);
            return Ok(example);
        }

        [HttpPost]
        //[HttpPost, Authorize]
        public async Task<IActionResult> Post([FromBody] ExampleViewModel exampleViewModel)
        {
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

        [HttpPut]
        //[HttpPut, Authorize]
        public async Task<IActionResult> Put([FromBody] ExampleViewModel exampleViewModel)
        {
            var responseExampleModel = await exampleService.UpdateExample(exampleViewModel);
            return Ok(responseExampleModel);
        }

        [HttpDelete("{exampleId}")]
        //[HttpDelete("{exampleId}"), Authorize]
        public async Task<IActionResult> Delete(string exampleId)
        {
            var isDeletionSuccessful = await exampleService.DeleteExample(exampleId);
            return Ok(isDeletionSuccessful);
        }
    }
}
