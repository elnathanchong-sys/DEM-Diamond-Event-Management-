global using DEM__Diamond_Event_Management_.Controllers;
using DEM__Diamond_Event_Management_;
using DEM__Diamond_Event_Management_.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSqlServer<DB>($@"
    Data Source=(LocalDB)\MSSQLLocalDB;
    AttachDbFilename={builder.Environment.ContentRootPath}\DEM_DB.mdf;
    Database=DEM_DB;
    Trusted_Connection=True;
    TrustServerCertificate=True;
");
// Used to enable cookies-based authentication service
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

// Allow our helper class to access to HTTP context later
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<Helper>();
var app = builder.Build();
//Added to properly route request
app.UseRouting();

//Added so that the system can actually remember someone
app.UseAuthentication();
app.UseAuthorization();

app.UseDeveloperExceptionPage();
app.UseHttpsRedirection();
app.UseStaticFiles();
//When entering the website the first page will be the login page.
app.MapControllerRoute(
    name:"default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
