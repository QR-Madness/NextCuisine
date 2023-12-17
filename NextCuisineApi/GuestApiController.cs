using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextCuisineApi.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NextCuisineApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestApiController(NextCuisineApiContext context) : ControllerBase
    {
        private readonly NextCuisineApiContext _context = context;
        private readonly AwsContext _awsContext = new();

        // GET: api/<GuestApiController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<GuestApiController>/5
        [HttpGet("{id}")]
        public async Task<ObjectResult> GetAsync(string id)
        {
            if (id == null || _context.Guest == null)
            {
                return this.BadRequest(id);
            }
            var guest = await _context.Guest.FirstOrDefaultAsync(m => m.Uid == id);
            if (guest == null)
            {
                return this.BadRequest(id);
            }
            return this.Ok(guest);
        }

        // POST api/<GuestApiController>
        [HttpPost]
        public void Post([FromBody] string value)
        {

        }

        // PUT api/<GuestApiController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {

        }

        // DELETE api/<GuestApiController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
