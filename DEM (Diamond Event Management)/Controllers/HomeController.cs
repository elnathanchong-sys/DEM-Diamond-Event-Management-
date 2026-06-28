using DEM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DEM.Controllers;

// TODO
public class HomeController(DB db) : Controller
{
    // GET: Home/Index
    public IActionResult Index()
    {
        var m = db.Registrations
                .Include(s => s.Customer);

        return View(m);
    }

    // GET: Home/Detail
    public IActionResult Detail(string? id)
    {
        var m = db.Registrations
                .Include(s => s.Event)
                .FirstOrDefault(s => s.RegID == id);

        if (m == null)
        {
            return RedirectToAction("Index");
        }

        return View(m);
    }
}
