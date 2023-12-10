
using System.Diagnostics;
using System.Security.Authentication;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NextCuisine.Data;
using NextCuisine.Models;

namespace NextCuisine.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AwsContext _awsContext = new();

        // GET: ProfileController
        public async Task<ActionResult> Index()
        {
            try
            {
                var uid = HttpContext.Session.GetString("uid") ?? throw new AuthenticationException();
                var profile = await _awsContext.GetProfile(uid);
                // create the user's profile if it doesn't exist yet
                if (profile != null) return View(profile);
                profile = new GuestProfile()
                {
                    Uid = uid
                };
                await _awsContext.CreateProfile(profile);
                return View(profile);
            }
            catch
            {
                return Redirect("/guests/login");
            }
        }

        // GET: ProfileController
        [HttpGet("/edit")]
        public async Task<ActionResult> EditPage()
        {
            try
            {
                var profile = await _awsContext.GetProfile(HttpContext.Session.GetString("uid") ?? throw new AuthenticationException());
                return profile == null ? Redirect("/uploads") : View("Edit", profile);
            }
            catch
            {
                return Redirect("/guests/login");
            }
        }

        // GET: ProfileController/Edit
        [HttpPost("/edit")]
        public async Task<IActionResult> EditPost([Bind("Name, Bio, FavoriteRecipes, FavoriteSnacks, GoodCombos")] GuestProfile modifiedProfile)
        {
            try
            {
                string uid = HttpContext.Session.GetString("uid") ?? throw new AuthenticationException();
                await _awsContext.EditProfile(uid, modifiedProfile);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                switch (ex)
                {
                    case AuthenticationException:
                        return Redirect("/guests/login");
                    default:
                        return View("Edit", modifiedProfile);
                }
            }
        }
    }
}
