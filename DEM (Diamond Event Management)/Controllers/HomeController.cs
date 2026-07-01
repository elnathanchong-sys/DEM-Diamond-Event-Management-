using DEM__Diamond_Event_Management_.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DEM.Controllers;

// TODO
public class HomeController(DB db) : Controller
{
    public IActionResult Index()
    {
        return View();
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
        return Index();
    }
}
