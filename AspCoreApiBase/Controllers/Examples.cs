using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AspCoreApiBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Examples : Controller
    {
        // GET: api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()//public IEnumerable<string> Get()
        {
            return new string[] { "Example1", "Example2" };
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
