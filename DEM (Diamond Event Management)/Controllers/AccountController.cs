using DEM__Diamond_Event_Management_.Models;
using Microsoft.AspNetCore.Mvc;

namespace DEM__Diamond_Event_Management_.Controllers;

public class AccountController(DB db, Helper hp) : Controller
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
    public IActionResult Login(LoginViewModel model, string? returnURL)
    {
        //Get user (admin or member) record based on email (PK)
        var u = db.Users.Find(model.Email);

        //Custom validation -> verify password
        if (u == null || !hp.VerifyPassword(u.HashedPassword, model.Password))
        {
            ModelState.AddModelError("", "Login failed. Please check your email and password.");
        }

        if (ModelState.IsValid)
        {
            TempData["Info"] = "Login successful!";

            //Sign In
            hp.SignIn(u!.Email, u.Role, model.RememberMe);

            //Redirect to Home Screen after sign in successful
            if (string.IsNullOrEmpty(returnURL))
            {
                return RedirectToAction("Index", "Home");
            }
            // No database implement yetttttttttt
            //if (model.Email == "admin@test.com" && model.Password == "password123")
            //{

            // Add an error message if wrong credentials 
            // Changed since practical uses another method instead of just another line of code under the login method
            //ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        return View(model);
    }

    public IActionResult Logout(string? returnURL)
    {
        TempData["Info"] = "Logout successful!";

        //Sign out
        hp.SignOut();

        return RedirectToAction("Index", "Home");
    }

    //this is where the error message will be shown
    public IActionResult AccessDenied(string? returnURL)
    {
        return View();
    }
}
