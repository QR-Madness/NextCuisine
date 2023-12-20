using Microsoft.AspNetCore.Mvc;
using NextCuisineApi.Data;
using NextCuisineApi.Models;
using System.Diagnostics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NextCuisineApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestUploadsApiController(NextCuisineApiContext context) : ControllerBase
    {
        private readonly NextCuisineApiContext _context = context;
        private readonly AwsContext _awsContext = new();

        // GET: api/<GuestUploadsApiController>
        [HttpGet]
        public async Task<ObjectResult> Get()
        {
            List<GuestUpload> publicUploads = await _awsContext.GetPublicUploads();
            return this.Ok(publicUploads);
        }

        [HttpGet("private-uploads")]
        public async Task<ObjectResult> GetPrivateUploads(string uid)
        {
            List<GuestUpload> privateUploads = await _awsContext.GetPrivateUploads(uid);
            return this.Ok(privateUploads);
        }

        [HttpGet("my-uploads")]
        public async Task<ObjectResult> GetMyUploads(string uid)
        {
            List<GuestUpload> privateUploads = await _awsContext.GetGuestUploads(uid);
            return this.Ok(privateUploads);
        }

        // GET api/<GuestUploadsApiController>/5
        [HttpGet("{id}")]
        public async Task<ObjectResult> GetAsync(string id)
        {
            GuestUpload upload = await _awsContext.GetUpload(id);
            return this.Ok(upload);
        }

        // POST api/<GuestUploadsApiController>
        [HttpPost]
        public async Task<OkResult> PostAsync([FromBody] GuestUpload upload)
        {
            await _awsContext.CreateUpload(upload);
            return this.Ok();
        }

        // PUT api/<GuestUploadsApiController>/5
        [HttpPut("{id}")]
        public async Task<OkResult> PutAsync(int id, [FromBody] GuestUpload upload)
        {
            await _awsContext.EditUpload(upload);
            return this.Ok();
        }

        // DELETE api/<GuestUploadsApiController>/5
        [HttpDelete("{id}")]
        public async Task<OkObjectResult> DeleteAsync(string id)
        {
            await _awsContext.DeleteUpload(await _awsContext.GetUpload(id));
            return this.Ok(200);
        }
    }
}
