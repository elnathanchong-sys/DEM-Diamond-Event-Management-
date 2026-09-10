using DEM__Diamond_Event_Management_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DEM__Diamond_Event_Management_.Controllers;

public class ProfileController(DB db) : Controller
{
    public async Task<IActionResult> Cusprofile(bool showAllUpcoming = false, bool showAllHistory = false)
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

        var paidRegistrationIds = await db.Payment
            .Select(p => p.RegistrationId)
            .Distinct()
            .ToListAsync();

        var upcomingEvents = registrations
            .Where(r => r.Event.StartDate >= DateOnly.FromDateTime(DateTime.Now))
            .Select(r => new ProfileEventViewModel
            {
                EventName = r.Event.Name,
                StartDate = r.Event.StartDate,
                StartTime = r.Event.StartTime,
                Venue = r.Event.VenueId,
                PosterImg = r.Event.PosterImg,
                RegistrationId = r.Id,
                HasPayment = paidRegistrationIds.Contains(r.Id)
            })
            .ToList();

        var purchases = registrations
            .Select(r => new ProfilePurchaseViewModel
            {
                EventName = r.Event.Name,
                RegistrationDate = r.RegDate,
                Amount = r.Event.Price,
                Status = r.Status,
                RegistrationId = r.Id,
                HasPayment = paidRegistrationIds.Contains(r.Id)
            })
            .ToList();

        ViewBag.ShowAllUpcoming = showAllUpcoming;
        ViewBag.ShowAllHistory = showAllHistory;
        ViewBag.HasMoreUpcoming = upcomingEvents.Count > 4;
        ViewBag.HasMorePurchases = purchases.Count > 4;

        // Create ViewModel
        var model = new ProfileViewModel
        {
            Email = member.Email,
            Name = member.Name,
            UserImg = member.UserImg,
            CreationDate = member.CreationDate,

            MembershipLevel = member.MembershipLevel,
            RewardPoints = member.RewardPoints,

            UpcomingEvents = showAllUpcoming ? upcomingEvents : upcomingEvents.Take(4).ToList(),
            Purchases = showAllHistory ? purchases : purchases.Take(4).ToList()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Receipt(string id)
    {
        var email = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login", "Account");
        }

        var payment = await db.Payment
            .Include(p => p.Registration)
            .ThenInclude(r => r.Event)
            .ThenInclude(e => e.Venue)
            .FirstOrDefaultAsync(p => p.RegistrationId == id && p.Registration.UserEmail == email);

        return payment == null ? NotFound() : View(payment);
    }
}