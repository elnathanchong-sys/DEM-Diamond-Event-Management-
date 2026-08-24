using Microsoft.AspNetCore.Mvc;

namespace YourProjectName.Controllers
{
    public class UserController : Controller
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

        public IActionResult CusProfile()
        {
            return View();
        }
    }
}