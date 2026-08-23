using DEM__Diamond_Event_Management_.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

namespace DEM__Diamond_Event_Management_.Controllers;

// TODO
public class HomeController(DB db) : Controller
{
    public IActionResult Index(string? Status)
    {
        Status ??= "Ongoing"; // Default status when the page is loaded

        ViewBag.CurrentStatus = Status;

        ViewBag.Status = db.Event
                           .Where(e => e.Status != "Hidden") // Keep status with the Hidden status out of the list
                           .Select(e => e.Status) // Instead of fetching all atrributes it fetches only the status attribute
                           .Distinct() // Instead of 50 ongoing status it list down just one unique string ongoing
                           .ToList(); //Executes the SQL query

        var model = db.Event
                      .Where(e => e.Status == Status || Status == null)
                      .Select(e => new EventVM
                      {
                          Id = e.Id,
                          Name = e.Name,
                          PosterImg = e.PosterImg,
                          Status = e.Status
                      })
                      .ToList();

        if (Request.IsAjax())
        {
            return PartialView("_Event", model);
        }

        return View(model);
    }

    public IActionResult EventList(string[] statuses, string[] categories)
    {
        ViewBag.SelectedStatuses = statuses ?? Array.Empty<string>();
        ViewBag.SelectedCategories = categories ?? Array.Empty<string>();

        //Status with event counts
        var statusNames = new[] { "Ongoing", "Upcoming", "Completed" };
        ViewBag.Status = statusNames.Select(s => new
        {
            Name = s,
            Count = db.Event.Count(e => e.Status == s)
        }).ToList();

        //Categories with event counts
        ViewBag.Category = db.Category.Select(c => new
        {
            Id = c.Id,
            Name = c.CategoryName,
            Count = db.Event.Count(e => e.CategoryId == c.Id && e.Status != "Hidden")
        }).ToList();

        // Filter
        var query = db.Event.Where(e => e.Status != "Hidden");

        if (statuses != null && statuses.Length > 0)
        {
            query = query.Where(e => statuses.Contains(e.Status));
        }

        if (categories != null && categories.Length > 0)
        {
            query = query.Where(e => categories.Contains(e.CategoryId));
        }

        var model = query.Select(e => new EventVM
        {
            Id = e.Id,
            Name = e.Name,
            PosterImg = e.PosterImg,
            Status = e.Status,
            CategoryId = e.CategoryId
        }).ToList();

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return PartialView("_EventList", model);
        }

        return View(model);
    }

    [Authorize]
    public IActionResult Both()
    {
        return View();
    }

    [Authorize(Roles = "Member")]
    public IActionResult Member()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Admin()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }
}
