using DEM__Diamond_Event_Management_.Models;
using Microsoft.AspNetCore.Mvc;

namespace DEM__Diamond_Event_Management_.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public IActionResult Login(LoginViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                // No database implement yetttttttttt
                if (model.Email == "admin@test.com" && model.Password == "password123")
                {
                    //  Redirect the user to the Home page
                    return RedirectToAction("Index", "Home");
                }

                // Add an error message if wrong credentials 
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            
            return View(model);
        }
    }
}
