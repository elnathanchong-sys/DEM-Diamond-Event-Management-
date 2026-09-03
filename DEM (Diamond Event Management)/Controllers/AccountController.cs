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
    public async Task<IActionResult> Login(LoginViewModel model, string? returnURL)
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

            // Redirect based on role if no returnURL is specified
            if (string.IsNullOrEmpty(returnURL))
            {
                // Check if the role is Admin (case-insensitive)
                if (u.Role != null && u.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("Index", "Admin"); // Directs to AdminController's Index action
                }
                else
                {
                    return RedirectToAction("Index", "Home"); // Directs to HomeController's Index action for members
                }
            }
            else if (Url.IsLocalUrl(returnURL))
            {
                return Redirect(returnURL);
            }
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
    public async Task<IActionResult> CreateAccount(CreateAccountVM model)
    {
        if (ModelState.IsValid("Email") &&
            db.User.Any(u => u.Email == model.Email))
        {
            ModelState.AddModelError("Email", "Duplicated Email.");
        }

        if (ModelState.IsValid)
        {
            db.User.Add(new Member
            {
                Email = model.Email,
                HashedPassword = hp.HashPassword(model.Password),
                Name = model.Name,
                Gender = model.Gender,

                CreationDate = DateOnly.FromDateTime(DateTime.Now),

                Phone = "",
                UserImg = "",
                OngoingEvent = "",
                UpcomingEvent = "",

                Country = "",
                State = "",
                City = "",
                HouseNo = "",
                Address = "",

                MembershipLevel = "Silver",
                RewardPoints = 0
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

    // GET: /Account/ForgotPassword
    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    // POST: /Account/ForgotPassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = db.User.Find(model.Email);
        if (user != null)
        {
            string token = Guid.NewGuid().ToString();

            var resetLink = Url.Action("ResetPassword", "Account",
                new { email = model.Email, token = token }, Request.Scheme);

            string body = $@"
            <h2>Password Reset Request</h2>
            <p>Click the link below to reset your password for Diamond Event Management:</p>
            <p><a href='{resetLink}'>Reset Password</a></p>";

            try
            {
                await hp.SendEmailAsync(model.Email, "Reset Password - Diamond Event Management", body);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to send email right now. Please check your network connection.");
                return View(model);
            }
        }

        // Returns the confirmation view directly
        return View("ForgotPasswordConfirmation");
    }

    // GET: /Account/ResetPassword
    [HttpGet]
    public IActionResult ResetPassword(string email, string token)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login");
        }

        return View(new ResetPasswordViewModel { Email = email, Token = token });
    }

    // POST: /Account/ResetPassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = db.User.Find(model.Email);
        if (user != null)
        {
            user.HashedPassword = hp.HashPassword(model.Password);
            await db.SaveChangesAsync();

            TempData["Info"] = "Password reset successfully! Please log in.";
            return RedirectToAction("Login");
        }

        ModelState.AddModelError("", "User not found or invalid request.");
        return View(model);
    }
}