using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FamilyHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaceServiceController : ControllerBase
    {
        private readonly ILogger<FaceServiceController> _logger;

        public FaceServiceController(ILogger<FaceServiceController> logger)
        {
            logger = _logger;
        }

        // GET: api/FaceService
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/FaceService/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/FaceService
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/FaceService/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
