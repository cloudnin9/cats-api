using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CorrelationId;

namespace cats_api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CatsController : ControllerBase
    {
        private readonly ICorrelationContextAccessor _correlationIdAccessor;

        public CatsController(ICorrelationContextAccessor correlationIdAccessor)
        {
            _correlationIdAccessor = correlationIdAccessor;    
        }
        // GET Cats
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "Daxter", "Mr Wiggle", "Doc", "Dot", $"{_correlationIdAccessor.CorrelationContext.Header} {_correlationIdAccessor.CorrelationContext.CorrelationId} {this.HttpContext.TraceIdentifier}" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(string name)
        {
            throw new ApplicationException("This is a test exception to see how the response would be handled");
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
