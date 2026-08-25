using DEM__Diamond_Event_Management_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DEM__Diamond_Event_Management_.Controllers;

public class RegistrationController(DB db) : Controller
{

    [HttpGet]
    public IActionResult RegistrationEvents(string? name, string[]? categories)
    {
        // For Checkbox
        ViewBag.SelectedCategories = categories ?? Array.Empty<string>();

        // Category list filter dropdown
        ViewBag.CategoryList = db.Category.Select(c => new
        {
            Id = c.Id,
            Name = c.CategoryName,
            Count = db.Event.Count(e => e.CategoryId == c.Id)
        }).ToList();

        var query = db.Event.AsQueryable();

        // search event name
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(e => e.Name != null && e.Name.Contains(name));
        }

        // filter by categories
        if (categories != null && categories.Length > 0)
        {
            query = query.Where(e => categories.Contains(e.CategoryId));
        }

        var model = query.Select(e => new EventApprovalVM
        {
            Id = e.Id,
            Name = e.Name,
            PosterImg = e.PosterImg,
            Status = e.Status,
            CategoryId = e.CategoryId,
            // Approval wait for admins decision whether the team gets in or not
            PendingCount = db.Registration.Count(r => r.EventId == e.Id &&
                                                       r.Status != "Approved" &&
                                                       r.Status != "Denied" &&
                                                       r.Status != "Cancelled")
        }).ToList();

        return View(model);
    }

    //------------------------------Registration Teams------------------------------------------------
    [HttpGet]
    public IActionResult RegistrationTeams(string? eventId)
    {
        var e = db.Event.Find(eventId);

        if (e == null)
        {
            return RedirectToAction("RegistrationEvents");
        }

        ViewBag.EventId = e.Id;
        ViewBag.EventName = e.Name;

        var model = db.Registration
                       .Where(r => r.EventId == eventId && r.Status != "Cancelled") // cancelled teams don't need a decision
                       .Select(r => new TeamRowVM
                       {
                           Id = r.Id,
                           TeamName = r.TeamName,
                           RegDate = r.RegDate,
                           Status = r.Status
                       })
                       .ToList();

        return View(model);
    }

    //-----------------------------Registration Details-------------------------
    [HttpGet]
    public IActionResult RegistrationDetails(string? id)
    {
        var r = db.Registration
                   .Include(x => x.Players)
                   .Include(x => x.Event)
                   .FirstOrDefault(x => x.Id == id);

        if (r == null)
        {
            return RedirectToAction("RegistrationEvents");
        }

        var model = new TeamDetailVM
        {
            Id = r.Id,
            TeamName = r.TeamName,
            RegDate = r.RegDate,
            Status = r.Status,
            UserEmail = r.UserEmail,
            EventId = r.EventId,
            EventName = r.Event != null ? r.Event.Name : "",
            Players = r.Players.Select(p => new PlayerRowVM
            {
                IGN = p.IGN,
                Role = p.Role
            }).ToList()
        };

        return View(model);
    }

    // For updating team status (POST) 
    [HttpPost]
    public IActionResult RegistrationUpdateStatus(string? id, string? status)
    {
        var r = db.Registration.Find(id);

        // Only allow the 3 admin decisions so the Status column doesn't end up with junk values
        if (r != null && (status == "Approved" || status == "Denied" || status == "Pending"))
        {
            r.Status = status;
            db.SaveChanges();

            TempData["Info"] = $"Team '{r.TeamName}' marked as {status}.";
        }

        return RedirectToAction("RegistrationDetails", new { id });
    }
}
