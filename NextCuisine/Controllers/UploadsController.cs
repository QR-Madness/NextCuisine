using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NextCuisine.Controllers
{
    public class UploadsController : Controller
    {
        // GET: UploadsController
        public ActionResult Index()
        {
            return View();
        }

        // GET: UploadsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UploadsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UploadsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: UploadsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UploadsController/Edit/5
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

        // GET: UploadsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UploadsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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
