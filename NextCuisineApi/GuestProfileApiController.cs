using Microsoft.AspNetCore.Mvc;
using NextCuisineApi.Data;
using NextCuisineApi.Models;
using System.Security.Authentication;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NextCuisineApi
{
    [Route("api/profile")]
    [ApiController]
    public class GuestProfileApiController(NextCuisineApiContext context) : ControllerBase
    {
        private readonly NextCuisineApiContext _context = context;
        private readonly AwsContext _awsContext = new();

        // GET: api/<GuestProfileApiController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<GuestProfileApiController>/5
        [HttpGet("{uid}")]
        public async Task<OkObjectResult> GetAsync(string uid)
        {
            var profile = await _awsContext.GetProfile(uid);
            // create the user's profile if it doesn't exist yet
            if (profile == null)
            {
                profile = new GuestProfile()
                {
                    Uid = uid
                };
                await _awsContext.CreateProfile(profile);
            }
            return this.Ok(profile);
        }

        // POST api/<GuestProfileApiController>
        [HttpPost]
        public async Task<OkResult> PostAsync([FromBody] GuestProfile profile)
        {
            await _awsContext.EditProfile(profile.Uid, profile);
            return this.Ok();
        }
    }
}
