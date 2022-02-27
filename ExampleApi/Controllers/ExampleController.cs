using ExampleApi.Services.Interfaces;
using ExampleApi.ViewModels;
//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExampleApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExampleController : Controller
    {
        IExampleService exampleService;
        private readonly ILogger<ExampleController> logger;

        public ExampleController(ILogger<ExampleController> logger, IExampleService exampleService)
        {
            this.logger = logger;
            this.exampleService = exampleService;
        }

        [HttpGet]
        //[HttpGet, Authorize]
        public async Task<IEnumerable<ExampleViewModel>> Get()
        {
            var returnList = await exampleService.GetExamples();
            return returnList;
        }

        [HttpPut]
        //[HttpPut, Authorize]
        public async Task<IActionResult> Put([FromBody] ExampleViewModel exampleViewModel)
        {
            var responseExampleModel = await exampleService.UpdateExample(exampleViewModel);
            return Ok(responseExampleModel);
        }
    }
}
