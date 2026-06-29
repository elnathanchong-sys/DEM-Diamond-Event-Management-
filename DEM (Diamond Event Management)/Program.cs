global using DEM__Diamond_Event_Management_.Controllers;
using DEM__Diamond_Event_Management_;
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

builder.Services.AddAuthentication().AddCookie();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<Helper>();
var app = builder.Build();
//Added to properly route request
app.UseRouting();

//Added so that the system can actually remember someone
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseStaticFiles();
//When entering the website the first page will be the login page.
app.MapControllerRoute(
    name:"default",
    pattern: "{controller=Account}/{action=Login}/{id?}"
);

app.Run();
