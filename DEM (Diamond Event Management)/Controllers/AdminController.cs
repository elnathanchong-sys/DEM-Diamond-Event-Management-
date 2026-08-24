using DEM__Diamond_Event_Management_.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DEM__Diamond_Event_Management_.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(DB db, Helper hp) : Controller
{
    // READ: Display all admins
    [HttpGet]
    public IActionResult ReadAdmin()
    {
        var admins = db.Admin.ToList();
        return View(admins);
    }

    // CREATE: GET
    [HttpGet]
    public IActionResult CreateAdmin()
    {
        return View();
    }

    // CREATE: POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAdmin(AdminCreateViewModel model)
    {
        if (db.User.Any(u => u.Email == model.Email))
        {
            ModelState.AddModelError("Email", "An account with this email address already exists.");
        }

        if (ModelState.IsValid)
        {
            var newAdmin = new Admin
            {
                Email = model.Email,
                HashedPassword = hp.HashPassword(model.Password),
                Name = model.Name,
                Gender = model.Gender,
                Phone = model.Phone ?? "",
                CreationDate = DateOnly.FromDateTime(DateTime.Now),
                UserImg = "",
                OngoingEvent = "",
                UpcomingEvent = ""
            };

            db.Admin.Add(newAdmin);
            await db.SaveChangesAsync();

            TempData["Info"] = "New admin created successfully.";
            return RedirectToAction(nameof(ReadAdmin));
        }

        return View(model);
    }

    // UPDATE: GET
    [HttpGet]
    public IActionResult UpdateAdmin(string email)
    {
        if (string.IsNullOrEmpty(email)) return NotFound();

        var admin = db.Admin.Find(email);
        if (admin == null) return NotFound();

        var model = new AdminEditViewModel
        {
            Email = admin.Email,
            Name = admin.Name,
            Gender = admin.Gender,
            Phone = admin.Phone
        };

        return View(model);
    }

    // UPDATE: POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateAdmin(AdminEditViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var admin = db.Admin.Find(model.Email);
        if (admin == null) return NotFound();

        admin.Name = model.Name;
        admin.Gender = model.Gender;
        admin.Phone = model.Phone ?? "";

        if (!string.IsNullOrEmpty(model.NewPassword))
        {
            admin.HashedPassword = hp.HashPassword(model.NewPassword);
        }

        await db.SaveChangesAsync();
        TempData["Info"] = "Admin profile updated successfully.";
        return RedirectToAction(nameof(ReadAdmin));
    }

    // DELETE: GET
    [HttpGet]
    public IActionResult DeleteAdmin(string email)
    {
        if (string.IsNullOrEmpty(email)) return NotFound();

        var admin = db.Admin.Find(email);
        if (admin == null) return NotFound();

        if (User.Identity?.Name == email)
        {
            TempData["Error"] = "You cannot delete your own admin account.";
            return RedirectToAction(nameof(ReadAdmin));
        }

        return View(admin);
    }

    // DELETE: POST
    [HttpPost, ActionName("DeleteAdmin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAdminConfirmed(string email)
    {
        if (User.Identity?.Name == email)
        {
            TempData["Error"] = "You cannot delete your own admin account.";
            return RedirectToAction(nameof(ReadAdmin));
        }

        var admin = db.Admin.Find(email);
        if (admin != null)
        {
            db.Admin.Remove(admin);
            await db.SaveChangesAsync();
            TempData["Info"] = "Admin account deleted successfully.";
        }

        return RedirectToAction(nameof(ReadAdmin));
    }
    
    [HttpGet]
    public IActionResult Index()
    {
        
        var model = new EventVM
        {
            
        };

        return View(model);
    }
}