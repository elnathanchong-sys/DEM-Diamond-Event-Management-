using DEM__Diamond_Event_Management_.Models;
using Microsoft.AspNetCore.Mvc;

namespace DEM__Diamond_Event_Management_.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        // This loads the empty form when the user navigates to the page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        // This processes the data when the user clicks "Log in"
        [HttpPost]
        [ValidateAntiForgeryToken] // Protects against Cross-Site Request Forgery
        public IActionResult Login(LoginViewModel model)
        {
            // 1. Check if the submitted data passes our Model validation rules
            if (ModelState.IsValid)
            {
                // 2. Dummy authentication logic (No database implemented yet)
                if (model.Email == "admin@test.com" && model.Password == "password123")
                {
                    // Success: Redirect the user to the Home page
                    return RedirectToAction("Index", "Home");
                }

                // Failure: Add an error message to display on the form
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            // 3. If we got this far, something failed. Redisplay the form with errors.
            return View(model);
        }
    }
}
