using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NextCuisine.Data;
using NextCuisine.Models;

namespace NextCuisine.Controllers
{
    public class GuestsController : Controller
    {
        private readonly NextCuisineContext _context;
        private readonly AwsContext _awsContext = new();

        public GuestsController(NextCuisineContext context)
        {
            _context = context;
        }

        // Authorize a guest's (user's) session
        private void GuestSessionCreate(string uid, string username)
        {
            HttpContext.Session.SetString("uid", uid);
            HttpContext.Session.SetString("username", username);
        }

        private void GuestSessionDelete()
        {
            HttpContext.Session.Remove("uid");
            HttpContext.Session.Remove("username");
        }

        // GET: Guests/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Guest == null)
            {
                return NotFound();
            }
            var guest = await _context.Guest
                .FirstOrDefaultAsync(m => m.Uid == id);
            if (guest == null)
            {
                return NotFound();
            }
            return View(guest);
        }

        // GET: Guests/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Guests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,Password,RecoveryEmail")] Guest guest)
        {
            // todo check for username conflictions
            if (ModelState.IsValid)
            {
                // add the user to RDS for authentication
                _context.Add(guest);
                // add an empty user profile

                await _context.SaveChangesAsync();
                GuestSessionCreate(guest.Uid, guest.Username);
                return RedirectToAction(nameof(Details));
            }
            return View(guest);
        }

        // GET: Guests/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Guests/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Username,Password")] Guest guest)
        {
            // todo Retrieve the user with the username + password
            Guest? guestMatch = _context.Guest.FirstOrDefault(g => g.Username == guest.Username && g.Password == guest.Password);
            // fail if not match is found
            Debug.WriteLine(guestMatch?.Username);
            if (guestMatch == null)
            {
                return View(guest);
            }
            // authenticate session (via cookies) for 24 hours
            GuestSessionCreate(guestMatch.Uid, guestMatch.Username);
            // redirect to main page
            return Redirect("/uploads");
        }

        [HttpGet("/logout")]
        public IActionResult Logout()
        {
            GuestSessionDelete();
            return Redirect("/");
        }

        // GET: Guests/Edit
        public async Task<IActionResult> Edit()
        {
            try
            {
                var id = HttpContext.Session.GetString("uid") ?? throw new AuthenticationException();
                var guest = await _context.Guest.FindAsync(id);
                if (guest == null)
                {
                    return RedirectToAction(nameof(Create));
                }
                if (guest.Uid != id)
                {
                    return Redirect("/");
                }
                return View(guest);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Login));
            }
        }

        // PATCH: Guests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Uid, Username,Password,RecoveryEmail")] Guest guest)
        {
            try
            {
                var uid = HttpContext.Session.GetString("uid") ?? throw new AuthenticationException();
                if (ModelState.IsValid && uid == guest.Uid)
                {
                    _context.Update(guest);
                    await _context.SaveChangesAsync();
                }
                ViewBag.Status = "Saved!";
                return View(guest);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Login));
            }
        }

        // GET: Guests/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Guest == null)
            {
                return NotFound();
            }

            if (id == HttpContext.Session.GetString("uid"))
            {
                var guest = await _context.Guest
                    .FirstOrDefaultAsync(m => m.Uid == id);
                if (guest == null)
                {
                    return NotFound();
                }
                return View(guest);
            }
            else
            {
                return Redirect("/");
            }
        }

        // POST: Guests/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed()
        {
            var id = HttpContext.Session.GetString("uid");
            var guest = await _context.Guest.FindAsync(id);
            if (guest != null && id == guest.Uid)
            {
                _context.Guest.Remove(guest);
            }

            await _context.SaveChangesAsync();
            // TODO Delete Profile and S3 Uploads
            return Redirect("/");
        }

        private bool GuestExists(string id)
        {
            return (_context.Guest?.Any(e => e.Uid == id)).GetValueOrDefault();
        }
    }
}
