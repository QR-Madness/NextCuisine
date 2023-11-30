using System.Collections.ObjectModel;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using NextCuisine.Data;
using NextCuisine.Models;

namespace NextCuisine.Controllers
{
    public class UploadsController : Controller
    {
        private readonly AwsContext _awsContext = new();

        // GET: UploadsController
        public ActionResult Index()
        {
            List<GuestUpload> publicUploads = _awsContext.GetPublicUploads();
            return View(publicUploads);
        }

        // GET: UploadsController/Details/5
        public ActionResult Details(string id)
        {
            // todo: check visibility settings and owner uid
            // todo: get object from context
            // todo: render page
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
        public async Task<ActionResult> Create([Bind("Title, ShortDescription, Content")] GuestUpload upload)
        {
            // TODO Upload files from input
            try
            {
                // set owner uid
                upload.OwnerUid = HttpContext.Session.GetString("uid") ?? throw new InvalidOperationException();
                ViewData["LoadText"] = "Uploading now...";
                // upload object
                await _awsContext.CreateUpload(upload);
                ViewData["LoadText"] = "Created!";
                // send user to ffed
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(upload);
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
