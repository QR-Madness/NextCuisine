
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NextCuisine.Controllers
{
    public class GuestProfileController : Controller
    {
        // GET: GuestProfileController
        public ActionResult Index()
        {
            return View();
        }

        // GET: GuestProfileController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: GuestProfileController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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
