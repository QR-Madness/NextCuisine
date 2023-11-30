using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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

        // GET: Guests
        public async Task<IActionResult> Index()
        {
            return _context.Guest != null ?
                        View(await _context.Guest.ToListAsync()) :
                        Problem("Entity set 'NextCuisineContext.Guest'  is null.");
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
                return RedirectToAction(nameof(Index));
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
            Guest? guestMatch =
                _context.Guest.FirstOrDefault(g => g.Username == guest.Username && g.Password == guest.Password);
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

        // GET: Guests/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Guest == null)
            {
                return NotFound();
            }

            var guest = await _context.Guest.FindAsync(id);
            if (guest == null)
            {
                return NotFound();
            }
            return View(guest);
        }

        // PATCH: Guests/Edit/5
        [HttpPatch]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Username,Password,RecoveryEmail")] Guest guest)
        {
            if (id != guest.Uid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(guest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GuestExists(guest.Uid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(guest);
        }

        // GET: Guests/Delete/5
        public async Task<IActionResult> Delete(string id)
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

        // POST: Guests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Guest == null)
            {
                return Problem("Entity set 'NextCuisineContext.Guest'  is null.");
            }
            var guest = await _context.Guest.FindAsync(id);
            if (guest != null)
            {
                _context.Guest.Remove(guest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GuestExists(string id)
        {
            return (_context.Guest?.Any(e => e.Uid == id)).GetValueOrDefault();
        }
    }
}
