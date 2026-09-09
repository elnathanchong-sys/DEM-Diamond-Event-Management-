using DEM__Diamond_Event_Management_;
using DEM__Diamond_Event_Management_.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DEM__Diamond_Event_Management_.Controllers;

public class EventController(DB db,
                            IWebHostEnvironment en,
                            Helper hp) : Controller
{
    [HttpGet]
    public IActionResult EventRead(string? name)
    {
        var query = db.Event.AsQueryable();

        //search event name
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(e => e.Name != null && e.Name.Contains(name));
        }

        var model = query.Select(e => new EventVM
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            StartTime = e.StartTime,
            EndTime = e.EndTime,
            Price = e.Price,
            PosterImg = e.PosterImg,
            BannerImg = e.BannerImg,
            Status = e.Status,
            TotalTickets = e.TotalTickets,
            TicketSold = e.TicketSold,
            MaxPlayersPerTeam = e.MaxPlayersPerTeam,
            MaxTeams = e.MaxTeams,
            PrizePool = e.PrizePool,
            CategoryId = e.CategoryId,
            VenueId = e.VenueId
        }).ToList();

        return View(model);
    }

    //--------------------------------Event individual page-------------------------
    public IActionResult EventIndividual(string? id)
    {
        // connect to the database
        var model = db.Event
                       .Include(e => e.Venue)
                       .Include(e => e.Category)
                       .FirstOrDefault(e => e.Id == id);

        // Check if the event exists if not the redirect back to home
        if (model == null)
        {
            return RedirectToAction("EventList", "Home");
        }

        // Count the number of registered teams for the current event and ignore any "Cancalled or hidden events" 
        int registeredCount = db.Registration.Count(r => r.EventId == id && r.Status != "Cancelled");
        ViewBag.RegisteredCount = registeredCount;
        ViewBag.IsFull = registeredCount >= model.MaxTeams;

        // Set the default value is false for every user who visits the page.
        bool alreadyRegistered = false;
        //then you check if the user is logged in or not
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            string? userEmail = User.FindFirstValue(ClaimTypes.NameIdentifier); //get user email from the logged in user visiting the page.
            alreadyRegistered = db.Registration.Any(r => r.EventId == id &&
                                                         r.UserEmail == userEmail &&
                                                         r.Status != "Cancelled");
            // then check if the user has already registered for the current events
        }
        ViewBag.AlreadyRegistered = alreadyRegistered; //temp storage for cshtml
        ViewBag.CanRegister = model.Status == "Upcoming";
        return View(model);
    }

    //-----------------------------End Event individual page-------------------------
    //-----------------------------Registration-------------------------
    [HttpPost]
    [Authorize(Roles = "Member")]
    public IActionResult RegisterTeam(RegistrationVM vm)
    {
        //Check if the user is logged in and get the email of the logged in user
        string? userEmail = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (User.Identity == null || !User.Identity.IsAuthenticated || string.IsNullOrEmpty(userEmail))
        {
            TempData["Error"] = "Please login before registering a team.";
            return RedirectToAction("EventIndividual", new { id = vm.EventId });
        }

        // connect to the database
        var e = db.Event.Find(vm.EventId);

        // Check if the event still exists
        if (e == null)
        {
            TempData["Error"] = "That event no longer exists.";
            return RedirectToAction("EventList", "Home");
        }

        if (e.Status != "Upcoming")
        {
            TempData["Error"] = "Sorry, this tournament is no longer open for registration.";
            return RedirectToAction("EventIndividual", new { id = vm.EventId });
        }

        // Team name input check and trimmed
        string teamName = (vm.TeamName ?? "").Trim();
        // For player in game names input, trimmed 
        List<string> playerIgns = (vm.PlayerIGNs ?? [])
                                    .Where(p => !string.IsNullOrWhiteSpace(p))
                                    .Select(p => p.Trim())
                                    .ToList();

        // Registration Flash messages
        if (teamName == "")
        {
            TempData["Error"] = "Please enter a team name.";
            return RedirectToAction("EventIndividual", new { id = vm.EventId });
        }

        if (playerIgns.Count == 0)
        {
            TempData["Error"] = "Please enter at least one player IGN.";
            return RedirectToAction("EventIndividual", new { id = vm.EventId });
        }

        if (playerIgns.Count > e.MaxPlayersPerTeam)
        {
            TempData["Error"] = $"You cannot register more than {e.MaxPlayersPerTeam} players for this tournament.";
            return RedirectToAction("EventIndividual", new { id = vm.EventId });
        }

        // Check if the account is already registered for the current event 
        bool alreadyRegistered = db.Registration.Any(r => r.EventId == vm.EventId &&
                                                               r.UserEmail == userEmail &&
                                                               r.Status != "Cancelled");
        if (alreadyRegistered)
        {
            TempData["Error"] = "You have already registered a team for this tournament.";
            return RedirectToAction("EventIndividual", new { id = vm.EventId });
        }

        // Chcek whether if the current event is already full.
        int registeredCount = db.Registration.Count(r => r.EventId == vm.EventId && r.Status != "Cancelled");
        if (registeredCount >= e.MaxTeams)
        {
            TempData["Error"] = "Sorry, registration for this tournament is already full.";
            return RedirectToAction("EventIndividual", new { id = vm.EventId });
        }

        //Auto assign unique Id to every new team for registrationID
        string regId = $"TR{(db.Registration.Count() + 1):D4}";

        var registration = new Registration
        {
            Id = regId,
            TeamName = teamName,
            RegDate = DateOnly.FromDateTime(DateTime.Now),
            Status = "Pending",
            UserEmail = userEmail,
            EventId = vm.EventId
        };

        db.Registration.Add(registration);
        db.SaveChanges(); //save now so RegistrationID exists before we attach players to it

        //Count players so that not every player gets the same id
        int playerSeq = db.Player.Count();
        for (int i = 0; i < playerIgns.Count; i++)
        {
            playerSeq++;

            // Assign role dynamically based on roster
            string role = "Player";
            if (i == 0) role = "Captain";
            else if (i == 5) role = "Substitute";
            else if (i == 6) role = "Coach";
            else if (i > 6) role = "Extra";

            registration.Players.Add(new Player
            {
                Id = $"TP{playerSeq:D4}",
                IGN = playerIgns[i],
                Role = role,
                TeamRegistrationId = regId
            });
        }

        db.SaveChanges();

        TempData["Info"] = "Team registered. Please wait for the admin to approve your registration before payment.";
        return RedirectToAction("EventIndividual", new { id = vm.EventId });
    }    //-----------------------------End of Registration-------------------------
    // GET: Home/CheckEventId 
    public bool CheckEventId(string eventId)
    {
        return !db.Event.Any(e => e.Id == eventId);
    }

    // GET: Home/CheckCategoryId
    public bool CheckCategoryId(string categoryId)
    {
        return db.Category.Any(c => c.Id == categoryId);
    }

    // GET: Home/CheckVenueId
    public bool CheckVenueId(string venueId)
    {
        return db.Venue.Any(v => v.Id == venueId);
    }

    //-----------------------------EventCreate---------------------------------------------

    // GET: Home/EventCreate
    public IActionResult EventCreate()
    {
        ViewBag.VenueList = new SelectList(db.Venue, "Id", "Id");
        ViewBag.CategoryList = new SelectList(db.Category.Where(c => c.Status != "Hidden"), "Id", "CategoryName");
        return View();

    }

    // POST: Home/EventCreate
    [HttpPost]
    public IActionResult EventCreate(EventVM vm, IFormFile PosterImg, IFormFile BannerImg)
    {

        if (ModelState.IsValid("PosterImg"))
        {
            var p = hp.ValidatePhoto(PosterImg);
            if (p != "") ModelState.AddModelError("PosterImg", p);
        }

        if (ModelState.IsValid("BannerImg"))
        {
            var b = hp.ValidatePhoto(BannerImg);
            if (b != "") ModelState.AddModelError("BannerImg", b);
        }

        if (ModelState.IsValid("CategoryId") &&
            !db.Category.Any(c => c.Id == vm.CategoryId))
        {
            ModelState.AddModelError("CategoryId", "Invalid Category ID");
        }

        if (ModelState.IsValid("VenueId") &&
            !db.Venue.Any(v => v.Id == vm.VenueId))
        {
            ModelState.AddModelError("VenueId", "Invalid Venue ID");
        }


        if (ModelState.IsValid)
        {
            string posterFilename = hp.SavePhoto(PosterImg, "uploads", 300, 425);
            string bannerFilename = hp.SavePhoto(BannerImg, "uploads", 7680, 2400);

            int nextEventNumber = db.Event
                         .AsEnumerable()
                         .Select(e => int.TryParse(e.Id.Substring(1), out int n) ? n : 0)
                         .DefaultIfEmpty(0)
                         .Max() + 1;
            string newEventId = $"E{nextEventNumber:D3}";

            db.Event.Add(new()
            {
                Id = newEventId,
                Name = vm.Name.Trim().ToUpper(),
                Description = vm.Description,
                StartDate = vm.StartDate,
                EndDate = vm.EndDate,
                StartTime = vm.StartTime,
                EndTime = vm.EndTime,
                Price = vm.Price,
                PosterImg = posterFilename,
                BannerImg = bannerFilename,
                Status = vm.Status,
                TotalTickets = 0,
                TicketSold = 0,
                MaxPlayersPerTeam = vm.MaxPlayersPerTeam,
                MaxTeams = vm.MaxTeams,
                PrizePool = vm.PrizePool,
                CategoryId = vm.CategoryId,
                VenueId = vm.VenueId
            });
            db.SaveChanges();
            TempData["Info"] = $"Record inserted. Event ID: {newEventId}";
            return RedirectToAction("EventRead");
        }

        ViewBag.VenueList = new SelectList(db.Venue, "Id", "Id", vm.VenueId);
        ViewBag.CategoryList = new SelectList(db.Category.Where(c => c.Status != "Hidden"), "Id", "CategoryName", vm.CategoryId);
        return View(vm);
    }
    //----------------------------End Event Create---------------------------------------------
    // GET: Home/Update
    [HttpGet]
    public IActionResult EventUpdate(string? id)
    {
        var e = db.Event.Find(id);

        if (e == null)
        {
            return RedirectToAction("EventRead");
        }

        var vm = new EventVM
        {
            Id = e.Id.Trim().ToUpper(),
            Name = e.Name.Trim().ToUpper(),
            Description = e.Description,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            StartTime = e.StartTime,
            EndTime = e.EndTime,
            Price = e.Price,
            PosterImg = e.PosterImg,
            BannerImg = e.BannerImg,
            Status = e.Status,
            TotalTickets = e.TotalTickets,
            TicketSold = e.TicketSold,
            MaxPlayersPerTeam = e.MaxPlayersPerTeam,
            MaxTeams = e.MaxTeams,
            PrizePool = e.PrizePool,
            CategoryId = e.CategoryId,
            VenueId = e.VenueId
        };

        ViewBag.VenueList = new SelectList(db.Venue, "Id", "Id");
        ViewBag.CategoryList = new SelectList(db.Category.Where(c => c.Status != "Hidden"), "Id", "CategoryName");
        return View(vm);

    }


    // POST: Home/Update
    [HttpPost]
    public IActionResult EventUpdate(EventVM vm, IFormFile? posterFile, IFormFile? bannerFile) //instead of Poster/BannerImg had to change to poster/bannerFile as it conflicts with EventVM
    {
        var e = db.Event.Find(vm.Id);

        if (e == null)
        {
            return RedirectToAction("EventRead");
        }

        // Validate the uploaded files only if the user 
        // actually chose a replacement file.
        if (posterFile != null && posterFile.Length > 0)
        {
            var p = hp.ValidatePhoto(posterFile);
            if (p != "") ModelState.AddModelError("PosterImg", p);
        }

        if (bannerFile != null && bannerFile.Length > 0)
        {
            var b = hp.ValidatePhoto(bannerFile);
            if (b != "") ModelState.AddModelError("BannerImg", b);
        }

        if (ModelState.IsValid("CategoryId") &&
            !db.Category.Any(p => p.Id == vm.CategoryId))
        {
            ModelState.AddModelError("CategoryId", "Invalid Category.");
        }

        if (ModelState.IsValid("VenueId") &&
            !db.Venue.Any(v => v.Id == vm.VenueId))
        {
            ModelState.AddModelError("VenueId", "Invalid Venue.");
        }


        if (ModelState.IsValid)
        {
            e.Name = vm.Name.Trim().ToUpper();
            e.Description = vm.Description;
            e.StartDate = vm.StartDate;
            e.EndDate = vm.EndDate;
            e.StartTime = vm.StartTime;
            e.EndTime = vm.EndTime;
            e.Price = vm.Price;
            e.Status = vm.Status;
            e.TotalTickets = vm.TotalTickets;
            e.TicketSold = vm.TicketSold;
            e.MaxPlayersPerTeam = vm.MaxPlayersPerTeam;
            e.MaxTeams = vm.MaxTeams;
            e.PrizePool = vm.PrizePool;
            e.CategoryId = vm.CategoryId;
            e.VenueId = vm.VenueId;


            // Only touch the photo on the database when a new one was uploaded.
            // Otherwise leave PosterImg and BannerImg filename as it is.
            if (posterFile != null && posterFile.Length > 0)
            {
                hp.DeletePhoto(e.PosterImg, "uploads");
                e.PosterImg = hp.SavePhoto(posterFile, "uploads", 600, 850);
            }

            if (bannerFile != null && bannerFile.Length > 0)
            {
                hp.DeletePhoto(e.BannerImg, "uploads");
                e.BannerImg = hp.SavePhoto(bannerFile, "uploads", 1200, 400);
            }

            db.SaveChanges();

            TempData["Info"] = "Record updated.";
            return RedirectToAction("EventRead");
        }

        ViewBag.VenueList = new SelectList(db.Venue, "Id", "Id");
        ViewBag.CategoryList = new SelectList(db.Category.Where(c => c.Status != "Hidden"), "Id", "CategoryName");
        return View(vm);
    }

    // POST: Home/Delete
    [HttpPost]
    public IActionResult EventDelete(string? id)
    {
        var e = db.Event.Find(id);

        if (e != null)
        {
            db.Event.Remove(e);
            db.SaveChanges();

            TempData["Info"] = "Record deleted.";
        }

        return Redirect(Request.Headers.Referer.ToString());
    }

    // GET: Home/Demo
    //    public IActionResult Demo()
    //    {
    //        var model = db.Event;
    //        return View(model);
    //    }


    // POST: Home/DeleteMany
    [HttpPost]
    public IActionResult DeleteMany(string[] ids)
    {
        int n = db.Event
                  .Where(s => ids.Contains(s.Id))
                  .ExecuteDelete();

        TempData["Info"] = $"{n} record(s) deleted.";
        return RedirectToAction("UNKOWNYET");
    }

    // POST: Home/Restore
    [HttpPost]
    public IActionResult Restore()
    {
        // (1) Delete all records
        db.Event.ExecuteDelete();

        // ------------------------------------------------

        // (2) Insert all records from "Event.txt"
        string path = Path.Combine(en.ContentRootPath, "Event.txt");

        foreach (string line in System.IO.File.ReadLines(path))
        {
            if (line.Trim() == "") continue;

            var data = line.Split("\t", StringSplitOptions.TrimEntries);

            db.Event.Add(new()
            {
                Id = data[0],
                Name = data[1],
                Description = data[2],
                StartDate = DateOnly.Parse(data[3]),
                EndDate = DateOnly.Parse(data[4]),
                StartTime = TimeOnly.Parse(data[5]),
                EndTime = TimeOnly.Parse(data[6]),
                Price = decimal.Parse(data[7]),
                PosterImg = data[8],
                BannerImg = data[9],
                Status = data[10],
                TotalTickets = int.Parse(data[11]),
                TicketSold = int.Parse(data[12]),
                MaxTeams = 16,
                MaxPlayersPerTeam = 7,
                PrizePool = 0,
                CategoryId = data[13],
                VenueId = data[14]
            });
        }

        db.SaveChanges();

        TempData["Info"] = "Record(s) restored.";
        return RedirectToAction("UNKOWNYET");
    }
}
