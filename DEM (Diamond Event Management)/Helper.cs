using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace DEM__Diamond_Event_Management_;

public class Helper(IWebHostEnvironment en, IHttpContextAccessor ct)
{

    public string ValidatePhoto(IFormFile f)
    {
        if (f == null || f.Length == 0) return "Please select a photo.";

        var reType = new Regex(@"^image\/(jpeg|jpg|png)$", RegexOptions.IgnoreCase);
        var reName = new Regex(@"^.+\.(jpeg|jpg|png)$", RegexOptions.IgnoreCase);

        if (!reType.IsMatch(f.ContentType) || !reName.IsMatch(f.FileName))
        {
            return "Only JPG, JPEG, and PNG photo is allowed.";
        }
        else if (f.Length > 100 * 1024 * 1024)
        {
            return "Photo size cannot more than 100MB.";
        }

        return "";
    }

    public string SavePhoto(IFormFile f, string folder, int width = 200, int height = 200)
    {
        var file = Guid.NewGuid().ToString("n") + ".jpg";
        var path = Path.Combine(en.WebRootPath, folder, file);

        var options = new ResizeOptions
        {
            Size = new(width, height),
            Mode = ResizeMode.Crop,
        };

        using var stream = f.OpenReadStream();
        using var img = Image.Load(stream);
        img.Mutate(x => x.Resize(options));
        img.Save(path);

        return file;
    }

    public void DeletePhoto(string file, string folder)
    {
        file = Path.GetFileName(file);
        var path = Path.Combine(en.WebRootPath, folder, file);
        File.Delete(path);
    }

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

