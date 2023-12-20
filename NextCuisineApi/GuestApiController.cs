using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextCuisineApi.Data;
using NextCuisineApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NextCuisineApi
{
    [Route("api/guests")]
    [ApiController]
    public class GuestApiController(NextCuisineApiContext context) : ControllerBase
    {
        private readonly NextCuisineApiContext _context = context;
        private readonly AwsContext _awsContext = new();

        // GET: api/<GuestApiController>
        [HttpGet]
        public ObjectResult Get()
        {
            return this.Ok(_context.Guest.ToList());
        }

        [HttpPost("login")]
        public ObjectResult Login([FromBody] Guest guest)
        {
            Guest? guestMatch = _context.Guest.FirstOrDefault(g => g.Username == guest.Username && g.Password == guest.Password);
            if (guestMatch == null)
            {
                return this.BadRequest(new BadRequestResult());
            }
            else
            {
                return this.Ok(guestMatch);
            }
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
        public async Task<OkResult> PostAsync([FromBody] Guest guest)
        {
            // edit the guest account
            _context.Update(guest);
            await _context.SaveChangesAsync();
            return this.Ok();
        }

        // PUT api/<GuestApiController>/5
        [HttpPut]
        public async Task<OkResult> PutAsync([FromBody] Guest guest)
        {
            // add the new user to the RDS
            _context.Add(guest);
            await _context.SaveChangesAsync();
            return this.Ok();
        }

        // DELETE api/<GuestApiController>/5
        [HttpDelete("{id}")]
        public async Task<OkResult> DeleteAsync(string id)
        {
            // delete the guest
            var guest = await _context.Guest.FindAsync(id);
            if (guest != null && id == guest.Uid)
            {
                _context.Guest.Remove(guest);
            }
            await _context.SaveChangesAsync();
            return this.Ok();
        }
    }
}
