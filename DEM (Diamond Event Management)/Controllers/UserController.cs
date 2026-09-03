using DEM__Diamond_Event_Management_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DEM__Diamond_Event_Management_.Controllers;

public class UserController(DB db) : Controller
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
            .Where(r => r.UserEmail == email)
            .OrderByDescending(r => r.RegDate)
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
                .Select(r => new ProfileEventViewModel
                {
                    EventName = r.Event.Name,
                    StartDate = r.Event.StartDate,
                    StartTime = r.Event.StartTime,
                    Venue = r.Event.VenueId,
                    PosterImg = r.Event.PosterImg,
                    RegistrationId = r.Id
                })
                .ToList(),

            Purchases = registrations
                .Select(r => new ProfilePurchaseViewModel
                {
                    EventName = r.Event.Name,
                    RegistrationDate = r.RegDate,
                    Amount = r.Event.Price,
                    Status = r.Status
                })
                .ToList()
        };

        return View(model);
    }
}