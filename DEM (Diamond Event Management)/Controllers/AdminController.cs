using DEM__Diamond_Event_Management_.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DEM__Diamond_Event_Management_.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(DB db, Helper hp) : Controller
{
    private const string SuperAdminEmail = "admin@domain.com";

    private bool IsSuperAdmin()
    {
        return hp.IsSuperAdmin();
    }

    private IActionResult? RequireSuperAdmin(string? returnAction = null)
    {
        if (!IsSuperAdmin())
        {
            TempData["Error"] = "Only the Super Admin can manage admin accounts.";
            return RedirectToAction(nameof(ReadAdmin));
        }

        return null;
    }

    [HttpGet]
    public async Task<IActionResult> EditMyProfile()
    {
        var email = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login", "Account");
        }

        var admin = await db.Admin
            .FirstOrDefaultAsync(a => a.Email == email);

        if (admin == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var model = new AdminEditViewModel
        {
            Email = admin.Email,
            Name = admin.Name,
            Gender = admin.Gender,
            Phone = admin.Phone,
            UserImg = admin.UserImg,
            NewPassword = string.Empty
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditMyProfile(AdminEditViewModel model, IFormFile? userPhoto)
    {
        var email = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login", "Account");
        }

        var admin = await db.Admin
            .FirstOrDefaultAsync(a => a.Email == email);

        if (admin == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (userPhoto != null && userPhoto.Length > 0)
        {
            if (userPhoto.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError("userPhoto", "Photo size cannot be more than 5 MB.");
                model.UserImg = admin.UserImg;
                return View(model);
            }

            var photoError = hp.ValidatePhoto(userPhoto);
            if (!string.IsNullOrEmpty(photoError))
            {
                ModelState.AddModelError("userPhoto", photoError);
                return View(model);
            }

            var newPhoto = hp.SavePhoto(userPhoto, "images");
            if (!string.IsNullOrEmpty(admin.UserImg))
            {
                hp.DeletePhoto(admin.UserImg, "images");
            }

            admin.UserImg = newPhoto;
        }

        admin.Name = model.Name;
        admin.Gender = model.Gender;
        admin.Phone = model.Phone ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(model.NewPassword))
        {
            admin.HashedPassword = hp.HashPassword(model.NewPassword);
        }

        await db.SaveChangesAsync();
        TempData["Info"] = "Your profile has been updated.";
        return RedirectToAction(nameof(Index));
    }

    // READ: Display all admins
    [HttpGet]
    public IActionResult ReadAdmin()
    {
        var admins = db.Admin
            .OrderBy(a => a.Name)
            .ToList();
        return View(admins);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BlockAdmin(string email)
    {
        var gate = RequireSuperAdmin();
        if (gate != null) return gate;

        if (string.IsNullOrWhiteSpace(email)) return NotFound();

        var currentEmail = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.Equals(email, currentEmail, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(email, SuperAdminEmail, StringComparison.OrdinalIgnoreCase))
        {
            TempData["Error"] = "The Super Admin account cannot be blocked.";
            return RedirectToAction(nameof(ReadAdmin));
        }

        var admin = await db.Admin.FirstOrDefaultAsync(a => a.Email == email);
        if (admin == null) return NotFound();

        if (!admin.IsBlocked)
        {
            admin.IsBlocked = true;
            await db.SaveChangesAsync();
        }

        TempData["Info"] = $"Admin '{admin.Name}' has been blocked.";
        return RedirectToAction(nameof(ReadAdmin));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UnblockAdmin(string email)
    {
        var gate = RequireSuperAdmin();
        if (gate != null) return gate;

        if (string.IsNullOrWhiteSpace(email)) return NotFound();

        var admin = await db.Admin.FirstOrDefaultAsync(a => a.Email == email);
        if (admin == null) return NotFound();

        if (admin.IsBlocked)
        {
            admin.IsBlocked = false;
            await db.SaveChangesAsync();
        }

        TempData["Info"] = $"Admin '{admin.Name}' has been unblocked.";
        return RedirectToAction(nameof(ReadAdmin));
    }

    // CREATE: GET
    [HttpGet]
    public IActionResult CreateAdmin()
    {
        var gate = RequireSuperAdmin();
        if (gate != null) return gate;

        return View();
    }

    // CREATE: POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAdmin(AdminCreateViewModel model)
    {
        var gate = RequireSuperAdmin();
        if (gate != null) return gate;

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
        var gate = RequireSuperAdmin();
        if (gate != null) return gate;

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
        var gate = RequireSuperAdmin();
        if (gate != null) return gate;

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

    // USER CRUD: list members
    [HttpGet]
    public IActionResult ReadUser()
    {
        var members = db.Member.OrderBy(m => m.Name).ToList();
        return View(members);
    }

    // USER CRUD: create GET
    [HttpGet]
    public IActionResult CreateUser()
    {
        var model = new AdminUserEditViewModel();
        return View(model);
    }

    // USER CRUD: create POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(AdminUserEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (db.User.Any(u => u.Email == model.Email))
        {
            ModelState.AddModelError("Email", "An account with this email address already exists.");
            return View(model);
        }

        var member = new Member
        {
            Email = model.Email,
            HashedPassword = hp.HashPassword(model.Password ?? "Member123"),
            Name = model.Name,
            Gender = model.Gender,
            Phone = model.Phone ?? string.Empty,
            CreationDate = DateOnly.FromDateTime(DateTime.Now),
            UserImg = string.Empty,
            OngoingEvent = string.Empty,
            UpcomingEvent = string.Empty,
            Country = model.Country ?? string.Empty,
            State = model.State ?? string.Empty,
            City = model.City ?? string.Empty,
            HouseNo = model.HouseNo ?? string.Empty,
            Address = model.Address ?? string.Empty,
            MembershipLevel = model.MembershipLevel ?? "Silver",
            RewardPoints = 0,
            IsBlocked = false
        };

        db.Member.Add(member);
        await db.SaveChangesAsync();

        TempData["Info"] = "Member account created successfully.";
        return RedirectToAction(nameof(ReadUser));
    }

    // USER CRUD: update GET
    [HttpGet]
    public IActionResult UpdateUser(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return NotFound();

        var member = db.Member.Find(email);
        if (member == null) return NotFound();

        var model = new AdminUserEditViewModel
        {
            Email = member.Email,
            Name = member.Name,
            Gender = member.Gender,
            Phone = member.Phone,
            Country = member.Country,
            State = member.State,
            City = member.City,
            HouseNo = member.HouseNo,
            Address = member.Address,
            MembershipLevel = member.MembershipLevel,
            IsBlocked = member.IsBlocked
        };

        return View(model);
    }

    // USER CRUD: update POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateUser(AdminUserEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var member = db.Member.Find(model.Email);
        if (member == null) return NotFound();

        member.Name = model.Name;
        member.Gender = model.Gender;
        member.Phone = model.Phone ?? string.Empty;
        member.Country = model.Country ?? string.Empty;
        member.State = model.State ?? string.Empty;
        member.City = model.City ?? string.Empty;
        member.HouseNo = model.HouseNo ?? string.Empty;
        member.Address = model.Address ?? string.Empty;
        member.MembershipLevel = model.MembershipLevel ?? member.MembershipLevel;

        if (!string.IsNullOrWhiteSpace(model.Password))
        {
            member.HashedPassword = hp.HashPassword(model.Password);
        }

        await db.SaveChangesAsync();
        TempData["Info"] = "Member account updated successfully.";
        return RedirectToAction(nameof(ReadUser));
    }

    // USER CRUD: block/unblock is the delete replacement
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BlockUser(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return NotFound();

        var member = db.Member.Find(email);
        if (member == null) return NotFound();

        member.IsBlocked = true;
        await db.SaveChangesAsync();
        TempData["Info"] = $"Member '{member.Name}' has been blocked.";

        return RedirectToAction(nameof(ReadUser));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UnblockUser(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return NotFound();

        var member = db.Member.Find(email);
        if (member == null) return NotFound();

        member.IsBlocked = false;
        await db.SaveChangesAsync();
        TempData["Info"] = $"Member '{member.Name}' has been unblocked.";

        return RedirectToAction(nameof(ReadUser));
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

        // Total teams waiting for admin approval
        int totalPending = db.Registration
            .Count(r => r.Status == "Pending");

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

        // Rank tournaments by the number of people in active registrations.
        var trendingEvents = db.Event
            .Select(e => new EventVM
            {
                Id = e.Id,
                Name = e.Name,
                StartDate = e.StartDate,
                Status = e.Status,
                RegisteredPeople = db.Registration
                    .Count(r => r.EventId == e.Id && r.Status != "Cancelled")
            })
            .OrderByDescending(e => e.RegisteredPeople)
            .ThenBy(e => e.Name)
            .Take(5)
            .ToList();

        var model = new EventVM
        {
            TotalEvents = totalEvents,
            TotalOngoing = totalOngoing,
            TotalPending = totalPending,
            TotalRevenue = totalRevenue,
            RecentEvents = recentEvents,
            TrendingEvents = trendingEvents
        };

        return View(model);
    }

    // ANNOUNCEMENTS & NOTIFICATIONS: GET
    [HttpGet]
    public IActionResult Notifications()
    {
        var model = new AdminNotificationDashboardVM();

        // 1. Fetch posted announcements from the database
        model.Announcements = db.Announcement
            .OrderByDescending(a => a.PostedAt)
            .Select(a => new AnnouncementItem
            {
                Id = a.Id,
                Title = a.Title,
                Message = a.Message,
                Type = a.Type,
                PostedAt = a.PostedAt
            })
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
            db.Announcement.Add(new Announcement
            {
                Title = title,
                Message = message,
                Type = type,
                PostedAt = DateTime.Now
            });

            db.SaveChanges();

            TempData["Info"] = "Announcement successfully posted!";
        }

        return RedirectToAction(nameof(Notifications));
    }
}