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

    public IActionResult EventList(string? Status)
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
