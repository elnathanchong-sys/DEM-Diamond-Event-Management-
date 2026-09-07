using DEM__Diamond_Event_Management_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DEM__Diamond_Event_Management_.Controllers;

public class ProfileController(DB db) : Controller
{
    public async Task<IActionResult> Cusprofile()
    {
        // Get email of the currently logged-in user
        var email = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // If user is not logged in
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login", "Account");
        }

        // Find the member in database
        var member = await db.Member
            .FirstOrDefaultAsync(m => m.Email == email);

        // Member not found
        if (member == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Get user's registrations and related event/venue
        var registrations = await db.Registration
            .Include(r => r.Event)
            .ThenInclude(e => e.Venue)
            .Where(r => r.UserEmail == email)
            .OrderByDescending(r => r.RegDate)
            .ToListAsync();

        // Create ViewModel
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