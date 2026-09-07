using DEM__Diamond_Event_Management_.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DEM__Diamond_Event_Management_.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VenueController : Controller
    {
        private readonly DB _context;

        public VenueController(DB context)
        {
            _context = context;
        }

        // READ
        public IActionResult Index()
        {
            var venues = _context.Venue.ToList();
            return View(venues);
        }

        // CREATE - Show form
        public IActionResult Create()
        {
            return View();
        }

        // CREATE - Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Venue venue)
        {
            if (_context.Venue.Any(v => v.Id == venue.Id))
            {
                ModelState.AddModelError("Id", "This Venue ID already exists. Please enter a different ID.");
            }

            if (!ModelState.IsValid)
            {
                return View(venue);
            }

            _context.Venue.Add(venue);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // UPDATE - Show form
        public IActionResult Edit(string id)
        {
            var venue = _context.Venue.Find(id);

            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // UPDATE - Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, Venue venue)
        {
            if (id != venue.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(venue);
            }

            _context.Venue.Update(venue);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // DELETE
        public IActionResult Delete(string id)
        {
            var venue = _context.Venue.Find(id);

            if (venue == null)
            {
                return NotFound();
            }

            _context.Venue.Remove(venue);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}