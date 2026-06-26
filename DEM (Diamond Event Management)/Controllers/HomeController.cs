using DEB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DEB.Controllers;

// TODO
public class HomeController(DB db) : Controller
{
    // GET: Home/Index
    public IActionResult Index()
    {
        var m = db.Students
                .Include(s => s.Program);

        return View(m);
    }

    // GET: Home/Detail
    public IActionResult Detail(string? id)
    {
        var m = db.Students
                .Include(s => s.Program)
                .FirstOrDefault(s => s.Id == id);

        if (m == null)
        {
            return RedirectToAction("Index");
        }

        return View(m);
    }
}
