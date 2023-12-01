using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        public async Task<ActionResult> Index()
        {
            List<GuestUpload> publicUploads = await _awsContext.GetPublicUploads();
            return View(publicUploads);
        }

        // GET: UploadsController
        [HttpGet("/private")]
        public async Task<ViewResult> PrivateUploads()
        {
            List<GuestUpload> publicUploads = await _awsContext.GetPrivateUploads(HttpContext.Session.GetString("uid") ?? throw new InvalidOperationException());
            return View("Index", publicUploads);
        }

        // GET: UploadsController
        [HttpGet("/me")]
        public async Task<ActionResult> MyUploads()
        {
            List<GuestUpload> guestUploads = await _awsContext.GetPrivateUploads(HttpContext.Session.GetString("uid") ?? throw new InvalidOperationException());
            return View("Index", guestUploads);
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
        public async Task<ActionResult> Create([Bind("Title, ShortDescription, Content, Visibility")] GuestUpload upload)
        {
            try
            {
                // set owner uid
                upload.OwnerUid = HttpContext.Session.GetString("uid") ?? throw new InvalidOperationException();
                ViewData["LoadText"] = "Uploading now...";
                // upload model object
                await _awsContext.CreateUpload(upload);
                // upload included files
                if (Request.Form.Files.Count > 0)
                {

                }
                ViewData["LoadText"] = "Created!";
                // send user to feed
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(upload);
            }
        }

        // GET: UploadsController/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            GuestUpload? upload = await _awsContext.GetUpload(id);
            if (upload == null || upload.OwnerUid != HttpContext.Session.GetString("uid"))
            {
                return RedirectToAction(nameof(Index));
            }
            return View(upload);
        }

        // POST: UploadsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, [Bind("Title, ShortDescription,Content,Visibility")] GuestUpload upload)
        {
            try
            {
                var originalUpload = await _awsContext.GetUpload(id);
                if (originalUpload == null || originalUpload.OwnerUid != HttpContext.Session.GetString("uid"))
                {
                    return RedirectToAction(nameof(Index));
                }
                Debug.WriteLine($"Edit upload {upload.Id}");
                await _awsContext.EditUpload(upload);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return View();
            }
        }

        // GET: UploadsController/Delete/5
        public async Task<RedirectToActionResult> Delete(string id)
        {
            var uploadObject = await _awsContext.GetUpload(id);
            if (uploadObject != null && uploadObject.OwnerUid == HttpContext.Session.GetString("uid"))
            {
                await _awsContext.DeleteUpload(id);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
