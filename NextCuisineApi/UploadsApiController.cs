using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NextCuisineApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadsApiController : ControllerBase
    {
        // GET: api/<UploadsApiController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UploadsApiController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UploadsApiController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UploadsApiController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UploadsApiController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
