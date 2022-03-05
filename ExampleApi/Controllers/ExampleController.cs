using ExampleApi.Services.Interfaces;
using ExampleApi.ViewModels;
//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
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
