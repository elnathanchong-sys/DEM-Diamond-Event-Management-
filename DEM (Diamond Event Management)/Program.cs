global using DEM__Diamond_Event_Management_.Controllers;
global using DEM__Diamond_Event_Management_;
global using DEM__Diamond_Event_Management_.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSqlServer<DB>($@"
    Data Source=(LocalDB)\MSSQLLocalDB;
    Database=DEM_DB;
    Trusted_Connection=True;
    TrustServerCertificate=True;
");
// Used to enable cookies-based authentication service
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// Allow our helper class to access to HTTP context later
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<Helper>();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DB>();
    var hp = scope.ServiceProvider.GetRequiredService<Helper>();

    await db.Database.MigrateAsync();

    string adminEmail = "admin@domain.com";


    if (!db.Admin.Any(a => a.Email == adminEmail))
    {
        var defaultAdmin = new Admin
        {
            Email = adminEmail,
            HashedPassword = hp.HashPassword("Admin12345"),
            Name = "Super Admin",
            Gender = "M",
            Phone = "0123456789",
            CreationDate = DateOnly.FromDateTime(DateTime.Now),
            UserImg = "",
            OngoingEvent = "",
            UpcomingEvent = ""
        };

        db.Admin.Add(defaultAdmin);
        db.SaveChanges();
    }
}
// ==========================================

app.UseDeveloperExceptionPage();
app.UseHttpsRedirection();
app.UseStaticFiles();

//Added to properly route request
app.UseRouting();

//Added so that the system can actually remember someone
app.UseAuthentication();

app.Use(async (context, next) =>
{
    if (context.User?.Identity?.IsAuthenticated == true)
    {
        var email = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrWhiteSpace(email))
        {
            var db = context.RequestServices.GetRequiredService<DB>();
            var user = await db.User.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null && user.IsBlocked)
            {
                context.Response.Redirect("/Account/AccessDenied");
                return;
            }
        }
    }

    await next();
});

app.UseAuthorization();

//When entering the website the first page will be the login page.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();