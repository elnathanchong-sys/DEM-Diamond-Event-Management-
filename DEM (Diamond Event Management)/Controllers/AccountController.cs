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
    public async Task <IActionResult> Login(LoginViewModel model, string? returnURL)
    {
        //Get user (admin or member) record based on email (PK)
        var u = db.User.Find(model.Email);

        //Custom validation -> verify password
        if (u == null || !hp.VerifyPassword(u.HashedPassword, model.Password))
        {
            ModelState.AddModelError("", "Login failed. Please check your email and password.");
        }

        if (ModelState.IsValid)
        {
            TempData["Info"] = "Login successful!";

            //Sign In
            await hp.SignIn(u!.Email, u.Role, model.RememberMe);

            //Redirect to Home Screen after sign in successful
            if (string.IsNullOrEmpty(returnURL))
            {
                return RedirectToAction("Index", "Home");
            }
            else if (Url.IsLocalUrl(returnURL))
            {
                return Redirect(returnURL);
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

    public async Task<IActionResult> Logout(string? returnURL)
    {
        TempData["Info"] = "Logout successful!";

        //Sign out
        await hp.SignOut();

        return RedirectToAction("Index", "Home");
    }

    //this is where the error message will be shown
    public IActionResult AccessDenied(string? returnURL)
    {
        return View();
    }

    //Account Register
    public IActionResult CreateAccount()
    {
        return View();
    }

    // POST: Account/Register
    [HttpPost]
    public async Task<IActionResult> CreateAccount(CreateAccountViewModel model)
    {
        if (ModelState.IsValid("Email") &&
            db.User.Any(u => u.Email == model.Email))
        {
            ModelState.AddModelError("Email", "Duplicated Email.");
        }

        //if (ModelState.IsValid("Photo"))
        //{
            //var err = hp.ValidatePhoto(vm.Photo);
            //if (err != "") ModelState.AddModelError("Photo", err);
        //}

        if (ModelState.IsValid)
        {
            db.User.Add(new()
            {
                Email = model.Email,
                HashedPassword = hp.HashPassword(model.Password),
                Name = model.Name,
                //PhotoURL = hp.SavePhoto(vm.Photo, "photo")
            });

            await db.SaveChangesAsync(); 

            TempData["Info"] = "Register successfully. Please login.";
            return RedirectToAction("Login");
        }

        return View(model);
    }

    public bool CheckEmail(string email)
    {
        return !db.User.Any(u => u.Email == email);
    }
}
