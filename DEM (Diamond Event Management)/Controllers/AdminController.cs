using DEM__Diamond_Event_Management_.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DEM__Diamond_Event_Management_.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(DB db, Helper hp) : Controller
{
    // In-memory list to persist custom announcements while the application is running
    private static readonly List<AnnouncementItem> _announcements = [];

    // READ: Display all admins
    [HttpGet]
    public IActionResult ReadAdmin()
    {
        var admins = db.Admin.ToList();
        return View(admins);
    }

    // CREATE: GET
    [HttpGet]
    public IActionResult CreateAdmin()
    {
        return View();
    }

    // CREATE: POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAdmin(AdminCreateViewModel model)
    {
        if (db.User.Any(u => u.Email == model.Email))
        {
            ModelState.AddModelError("Email", "An account with this email address already exists.");
        }

        if (ModelState.IsValid)
        {
            var newAdmin = new Admin
            {
                Email = model.Email,
                HashedPassword = hp.HashPassword(model.Password),
                Name = model.Name,
                Gender = model.Gender,
                Phone = model.Phone ?? "",
                CreationDate = DateOnly.FromDateTime(DateTime.Now),
                UserImg = "",
                OngoingEvent = "",
                UpcomingEvent = ""
            };

            db.Admin.Add(newAdmin);
            await db.SaveChangesAsync();

            TempData["Info"] = "New admin created successfully.";
            return RedirectToAction(nameof(ReadAdmin));
        }

        return View(model);
    }

    // UPDATE: GET
    [HttpGet]
    public IActionResult UpdateAdmin(string email)
    {
        if (string.IsNullOrEmpty(email)) return NotFound();

        var admin = db.Admin.Find(email);
        if (admin == null) return NotFound();

        var model = new AdminEditViewModel
        {
            Email = admin.Email,
            Name = admin.Name,
            Gender = admin.Gender,
            Phone = admin.Phone
        };

        return View(model);
    }

    // UPDATE: POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateAdmin(AdminEditViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var admin = db.Admin.Find(model.Email);
        if (admin == null) return NotFound();

        admin.Name = model.Name;
        admin.Gender = model.Gender;
        admin.Phone = model.Phone ?? "";

        if (!string.IsNullOrEmpty(model.NewPassword))
        {
            admin.HashedPassword = hp.HashPassword(model.NewPassword);
        }

        await db.SaveChangesAsync();
        TempData["Info"] = "Admin profile updated successfully.";
        return RedirectToAction(nameof(ReadAdmin));
    }

    // DELETE: GET
    [HttpGet]
    public IActionResult DeleteAdmin(string email)
    {
        if (string.IsNullOrEmpty(email)) return NotFound();

        var admin = db.Admin.Find(email);
        if (admin == null) return NotFound();

        if (User.Identity?.Name == email)
        {
            TempData["Error"] = "You cannot delete your own admin account.";
            return RedirectToAction(nameof(ReadAdmin));
        }

        return View(admin);
    }

    // DELETE: POST
    [HttpPost, ActionName("DeleteAdmin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAdminConfirmed(string email)
    {
        if (User.Identity?.Name == email)
        {
            TempData["Error"] = "You cannot delete your own admin account.";
            return RedirectToAction(nameof(ReadAdmin));
        }

        var admin = db.Admin.Find(email);
        if (admin != null)
        {
            db.Admin.Remove(admin);
            await db.SaveChangesAsync();
            TempData["Info"] = "Admin account deleted successfully.";
        }

        return RedirectToAction(nameof(ReadAdmin));
    }

    [HttpGet]
    [HttpGet]
    public IActionResult Index()
    {
        // Total number of events
        int totalEvents = db.Event.Count();

        // Total ongoing events
        int totalOngoing = db.Event
            .Count(e => e.Status == "Ongoing");

        // Total pending events
        int totalPending = db.Event
            .Count(e => e.Status == "Pending");

        // Total revenue from payments
        decimal totalRevenue = db.Payment
            .Sum(p => (decimal?)p.AmountPaid) ?? 0;

        // Get 5 most recent events
        var recentEvents = db.Event
            .OrderByDescending(e => e.StartDate)
            .Take(5)
            .Select(e => new EventVM
            {
                Id = e.Id,
                Name = e.Name,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Status = e.Status
            })
            .ToList();

        var model = new EventVM
        {
            TotalEvents = totalEvents,
            TotalOngoing = totalOngoing,
            TotalPending = totalPending,
            TotalRevenue = totalRevenue,
            RecentEvents = recentEvents
        };

        return View(model);
    }

    // ANNOUNCEMENTS & NOTIFICATIONS: GET
    [HttpGet]
    public IActionResult Notifications()
    {
        var model = new AdminNotificationDashboardVM();

        // 1. Fetch posted in-memory announcements
        model.Announcements = _announcements
            .OrderByDescending(a => a.PostedAt)
            .ToList();

        // 2. Fetch event schedule timings directly from DB[cite: 10]
        model.ScheduleUpdates = db.Event
            .Select(e => new ScheduleNotificationItem
            {
                EventId = e.Id,
                EventName = e.Name,
                StartDate = e.StartDate,
                StartTime = e.StartTime,
                EndDate = e.EndDate,
                EndTime = e.EndTime,
                Status = e.Status
            })
            .ToList();

        // 3. Fetch registration status logs directly from DB[cite: 10]
        var eventRegistrations = db.Registration
            .Include(r => r.Event)
            .Where(r => r.Status == "Approved" || r.Status == "Denied" || r.Status == "Rejected")
            .Select(r => new RegistrationNotificationItem
            {
                TeamName = r.TeamName,
                EventOrTournamentName = r.Event != null ? r.Event.Name : "Event",
                Status = r.Status,
                DateInfo = r.RegDate.ToString("dd MMM yyyy")
            })
            .ToList();

        var tournamentRegistrations = db.TournamentRegistration
            .Include(tr => tr.Tournament)
            .Include(tr => tr.Team)
            .Where(tr => tr.Status != RegistrationStatus.Pending)
            .Select(tr => new RegistrationNotificationItem
            {
                TeamName = tr.Team != null ? tr.Team.Name : "Team",
                EventOrTournamentName = tr.Tournament != null ? tr.Tournament.Name : "Tournament",
                Status = tr.Status.ToString(),
                AdminNote = tr.AdminNote,
                DateInfo = tr.ReviewedAt.HasValue
                    ? tr.ReviewedAt.Value.ToString("dd MMM yyyy")
                    : tr.RegisteredAt.ToString("dd MMM yyyy")
            })
            .ToList();

        model.RegistrationLogs = eventRegistrations.Concat(tournamentRegistrations).ToList();

        return View(model);
    }

    // ANNOUNCEMENTS & NOTIFICATIONS: POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult PostAnnouncement(string title, string message, string type)
    {
        if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(message))
        {
            _announcements.Add(new AnnouncementItem
            {
                Id = _announcements.Count + 1,
                Title = title,
                Message = message,
                Type = type,
                PostedAt = DateTime.Now
            });

            TempData["Info"] = "Announcement successfully posted!";
        }

        return RedirectToAction(nameof(Notifications));
    }
}