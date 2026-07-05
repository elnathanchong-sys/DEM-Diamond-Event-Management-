using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace DEM__Diamond_Event_Management_;

public class Helper(IWebHostEnvironment en, IHttpContextAccessor ct)
{
    // Password Hasher
    private readonly PasswordHasher<object> ph = new();
    public string HashPassword(string password)
    {
        return ph.HashPassword(0, password);
    }

    public bool VerifyPassword(string hash, string password)
    {
        return ph.VerifyHashedPassword(0, hash, password)
               == PasswordVerificationResult.Success;
    }

    public async Task SignIn(string email, string role, bool rememberMe)
    {   
        //Claim, identity and principal
        List<Claim> claims =
        [
            new(ClaimTypes.Name, email), //Store user PK
            new(ClaimTypes.Role, role), //Store user role (admin or member)
        ];

        ClaimsIdentity identity = new(claims, "Cookies");

        ClaimsPrincipal principal = new(identity);

        //Remember Me
        AuthenticationProperties properties = new()
        {
            IsPersistent = rememberMe,
            //Interesting it was auto added when I tabbed it. This could be an additional feature if needed?
            //!!!!ExpiresUtc = DateTime.UtcNow.AddDays(7),
        };

        //sign in
        await ct.HttpContext!.SignInAsync(principal, properties);
    }

    //Sign out
    public async Task SignOut()
    {
       await ct.HttpContext!.SignOutAsync();
    }

    public string RandomPassword()
    {
        string s = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string password = "";

        Random r = new();

        for (int i = 1; i <= 10; i++)
        {
            password += s[r.Next(s.Length)];
        }

        return password;
    }
}
