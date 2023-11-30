
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NextCuisine.Controllers
{
    public class ProfileController : Controller
    {
        // GET: ProfileController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ProfileController/Edit/5
        public IActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProfileController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUpload(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
