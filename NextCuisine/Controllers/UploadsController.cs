using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using NextCuisine.Data;
using NextCuisine.Models;
using NextCuisine.Tools;
using NuGet.Versioning;

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
        [HttpGet]
        public async Task<ViewResult> PrivateUploads()
        {
            List<GuestUpload> publicUploads = await _awsContext.GetPrivateUploads(HttpContext.Session.GetString("uid") ?? throw new InvalidOperationException());
            return View("Index", publicUploads);
        }

        // GET: UploadsController
        [HttpGet]
        public async Task<ActionResult> MyUploads()
        {
            List<GuestUpload> guestUploads = await _awsContext.GetGuestUploads(HttpContext.Session.GetString("uid") ?? throw new InvalidOperationException());
            return View("Index", guestUploads);
        }

        // GET: UploadsController/<Upload_Id>
        [HttpGet]
        public async Task<ActionResult> Details(string id)
        {
            try
            {
                GuestUpload upload = await _awsContext.GetUpload(id) ?? throw new NullReferenceException();
                switch (upload.Visibility)
                {
                    case "Private" when HttpContext.Session.GetString("uid") == upload.OwnerUid:
                        return View(upload);
                    case "Public":
                        return View(upload);
                    default:
                        return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex) { return RedirectToAction(nameof(Index)); }
        }

        [HttpPost]
        public async Task<ActionResult> DetailsFeedback(string id)
        {
            try
            {
                // authenticate via session
                var uid = HttpContext.Session.GetString("uid") ?? throw new BadHttpRequestException("Not logged in");
                var guestProfile = await _awsContext.GetProfile(uid) ?? throw new BadHttpRequestException("Can't find user profile");
                // populate object 
                string feedbackContent = Request.Form["feedback"];
                string feedbackRating = Request.Form["feedback-rating"];
                GuestUploadFeedback feedback = new GuestUploadFeedback
                {
                    OwnerUid = uid,
                    OwnerName = guestProfile.Name ?? "Anonymous User",
                    Content = feedbackContent,
                    Rating = feedbackRating
                };
                // save changes
                Debug.WriteLine($"Owner UID: {uid} | Content + Rating = {feedbackContent + feedbackRating.ToString()}");
                await _awsContext.PostUploadFeedback(id, feedback);
                // refresh upload view
                return Redirect($"/uploads/details/{id}");
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }


        // GET: Uploads/<UploadId>/download => Download Stream
        [HttpGet("/download/{id}/{fileId}")]
        public async Task<IActionResult> Download(string id, string fileId)
        {
            GuestUpload upload = await _awsContext.GetUpload(id) ?? throw new NullReferenceException();
            GuestUploadFile uploadFile = upload.Files.Find(u => u.Id == fileId) ?? throw new InvalidOperationException();
            Debug.WriteLine($"Upload: {uploadFile.Filename} :: Stored in S3 as {uploadFile.FilenameS3}");
            return _awsContext.DownloadGuestFile(uploadFile);
        }

        public async Task<IActionResult> ViewUploadFile(string id, string fileId)
        {
            GuestUpload upload = await _awsContext.GetUpload(id) ?? throw new NullReferenceException();
            GuestUploadFile uploadFile = upload.Files.Find(u => u.Id == fileId) ?? throw new InvalidOperationException();
            //string mimeType = "application/pdf";
            //Response.Headers.Append("Content-Disposition", "inline; filename=" + uploadFile.Filename);
            return await _awsContext.GetGuestFile(uploadFile);
        }

        // GET: UploadsController/Create
        [HttpGet]
        public ActionResult Create()
        {
            var upload = new GuestUpload();
            return View(upload);
        }

        // POST: UploadsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Title, ShortDescription, Content,Visibility")] GuestUpload upload)
        {
            try
            {
                // set owner uid
                upload.OwnerUid = HttpContext.Session.GetString("uid") ?? throw new InvalidOperationException();
                ViewData["LoadText"] = "Uploading now...";
                // upload included files
                if (Request.Form.Files.Count > 0)
                {
                    foreach (IFormFile file in Request.Form.Files)
                    {
                        GuestUploadFile guestUploadFile = new GuestUploadFile()
                        {
                            Filename = file.FileName.Normalize(),
                            FilenameS3 = DataTools.RandomString(15),
                            UploadDateTime = default
                        };
                        // upload to aws blob storage and link to the upload files
                        await _awsContext.UploadGuestFile(guestUploadFile, file.OpenReadStream());
                        upload.Files.Add(guestUploadFile);
                    }
                }
                // upload model object
                await _awsContext.CreateUpload(upload);
                ViewData["LoadText"] = "Created!";
                // send user to feed
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                ViewData["LoadText"] = "An error occurred.";
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
        public async Task<ActionResult> Edit(string id, [Bind("Title,ShortDescription,Content,Visibility")] GuestUpload upload)
        {
            try
            {
                // use the original upload to reflect changes
                var originalUpload = await _awsContext.GetUpload(id);
                originalUpload.Title = upload.Title;
                originalUpload.Content = upload.Content;
                originalUpload.ShortDescription = upload.ShortDescription;
                originalUpload.Visibility = upload.Visibility;
                // verify ownership
                if (originalUpload == null || originalUpload.OwnerUid != HttpContext.Session.GetString("uid"))
                {
                    return RedirectToAction(nameof(Index));
                }
                // upload only new files
                if (Request.Form.Files.Count > 0)
                {
                    foreach (IFormFile file in Request.Form.Files)
                    {
                        GuestUploadFile guestUploadFile = new GuestUploadFile()
                        {
                            Filename = file.FileName.Normalize(),
                            FilenameS3 = DataTools.RandomString(15),
                            UploadDateTime = default
                        };
                        // upload to aws blob storage and link to the upload files
                        await _awsContext.UploadGuestFile(guestUploadFile, file.OpenReadStream());
                        originalUpload.Files.Add(guestUploadFile);
                    }
                }
                // commit object and redirect to feed
                await _awsContext.EditUpload(originalUpload);
                return View("Details", upload);
            }
            catch (Exception ex)
            {
                // failed submission; send back with object
                Debug.WriteLine(ex);
                return View(upload);
            }
        }

        // GET: UploadsController/Delete/5
        [HttpGet]
        public async Task<RedirectToActionResult> Delete(string id)
        {
            var uploadObject = await _awsContext.GetUpload(id);
            if (uploadObject != null && uploadObject.OwnerUid == HttpContext.Session.GetString("uid"))
            {
                await _awsContext.DeleteUpload(await _awsContext.GetUpload(id));
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: UploadsController/DeleteFile/<FileId>
        [HttpGet("/Uploads/DeleteFile/{fileId}")]
        public async Task<IActionResult> DeleteFile(string fileId)
        {
            try
            {
                // find upload and verify ownership
                var upload = await _awsContext.GetUploadByFileId(fileId);
                if (upload != null && upload.OwnerUid == HttpContext.Session.GetString("uid"))
                {
                    // delete file
                    await _awsContext.DeleteUploadFile(fileId);
                }
                return Redirect($"/uploads/edit/{upload?.Id}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
