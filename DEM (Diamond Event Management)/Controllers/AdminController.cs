using Microsoft.AspNetCore.Mvc;

namespace DEM__Diamond_Event_Management_.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
