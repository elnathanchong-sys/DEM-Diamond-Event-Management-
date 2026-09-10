using DEM__Diamond_Event_Management_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DEM__Diamond_Event_Management_.Controllers;

public class UserController(DB db, Helper hp) : Controller
{
    public IActionResult Profile()
    {
        return View();
    }

    public IActionResult Edit()
    {
        return View();
    }

    public IActionResult Settings()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        var email = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login", "Account");
        }

        var member = await db.Member
            .FirstOrDefaultAsync(m => m.Email == email);

        if (member == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var model = new EditProfileViewModel
        {
            Email = member.Email,
            Name = member.Name,
            Phone = member.Phone,
            Gender = member.Gender,
            Country = member.Country,
            State = member.State,
            City = member.City,
            HouseNo = member.HouseNo,
            Address = member.Address,
            UserImg = member.UserImg
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(EditProfileViewModel model, IFormFile? userPhoto)
    {
        var email = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login", "Account");
        }

        var member = await db.Member
            .FirstOrDefaultAsync(m => m.Email == email);

        if (member == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        member.Name = model.Name;
        member.Phone = model.Phone;
        member.Gender = model.Gender;
        member.Country = model.Country;
        member.State = model.State;
        member.City = model.City;
        member.HouseNo = model.HouseNo;
        member.Address = model.Address;

        if (userPhoto != null && userPhoto.Length > 0)
        {
            var photoError = hp.ValidatePhoto(userPhoto);
            if (!string.IsNullOrEmpty(photoError))
            {
                ModelState.AddModelError("", photoError);
                return View(model);
            }

            var newPhoto = hp.SavePhoto(userPhoto, "images");
            if (!string.IsNullOrEmpty(member.UserImg))
            {
                hp.DeletePhoto(member.UserImg, "images");
            }

            member.UserImg = newPhoto;
        }

        await db.SaveChangesAsync();
        TempData["Info"] = "Profile updated successfully.";
        return RedirectToAction(nameof(CusProfile));
    }

    // Customer Profile
    public async Task<IActionResult> CusProfile()
    {
        // Get the email of the logged-in user
        var email = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // If user is not logged in
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login", "Account");
        }

        // Find the member in the database
        var member = await db.Member
            .FirstOrDefaultAsync(m => m.Email == email);

        // If member cannot be found
        if (member == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Get the member's registrations
        var registrations = await db.Registration
            .Include(r => r.Event)
            .ThenInclude(e => e.Venue)
            .Where(r => r.UserEmail == email)
            .OrderByDescending(r => r.RegDate)
            .ToListAsync();

        var paidRegistrationIds = await db.Payment
            .Select(p => p.RegistrationId)
            .Distinct()
            .ToListAsync();

        // Put database data into ProfileViewModel
        var model = new ProfileViewModel
        {
            Email = member.Email,
            Name = member.Name,
            UserImg = member.UserImg,
            CreationDate = member.CreationDate,

            MembershipLevel = member.MembershipLevel,
            RewardPoints = member.RewardPoints,

            UpcomingEvents = registrations
                .Where(r => r.Event.StartDate >= DateOnly.FromDateTime(DateTime.Now))
                .OrderBy(r => r.Event.StartDate)
                .Select(r => new ProfileEventViewModel
                {
                    EventName = r.Event.Name,
                    StartDate = r.Event.StartDate,
                    StartTime = r.Event.StartTime,
                    Venue = r.Event.Venue != null ? r.Event.Venue.VenueName : r.Event.VenueId,
                    PosterImg = r.Event.PosterImg,
                    RegistrationId = r.Id,
                    HasPayment = paidRegistrationIds.Contains(r.Id)
                })
                .ToList(),

            Purchases = registrations
                .OrderByDescending(r => r.RegDate)
                .Select(r => new ProfilePurchaseViewModel
                {
                    EventName = r.Event.Name,
                    RegistrationDate = r.RegDate,
                    Amount = r.Event.Price,
                    Status = r.Status,
                    RegistrationId = r.Id,
                    HasPayment = paidRegistrationIds.Contains(r.Id)
                })
                .ToList()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> MyRegisteredEvents()
    {
        var email = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login", "Account");
        }

        var registrations = await db.Registration
            .Include(r => r.Event)
            .ThenInclude(e => e.Venue)
            .Where(r => r.UserEmail == email)
            .OrderBy(r => r.Event.StartDate)
            .ToListAsync();

        var paidRegistrationIds = await db.Payment
            .Where(p => registrations.Select(r => r.Id).Contains(p.RegistrationId))
            .Select(p => p.RegistrationId)
            .Distinct()
            .ToListAsync();

        var model = registrations
            .Select(r => new ProfileEventViewModel
            {
                EventName = r.Event.Name,
                StartDate = r.Event.StartDate,
                StartTime = r.Event.StartTime,
                Venue = r.Event.Venue != null ? r.Event.Venue.VenueName : r.Event.VenueId,
                PosterImg = r.Event.PosterImg,
                RegistrationId = r.Id,
                HasPayment = paidRegistrationIds.Contains(r.Id)
            })
            .ToList();

        return View("MyRegisteredEvents", model);
    }
}