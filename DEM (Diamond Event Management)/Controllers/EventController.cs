using DEM__Diamond_Event_Management_;
using DEM__Diamond_Event_Management_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DEM__Diamond_Event_Management_.Controllers;

public class EventController(DB db, 
                            IWebHostEnvironment en) : Controller
{
    [HttpGet]
    public IActionResult EventRead()
    {
        return View();
    }

    // GET: Home/Detail
    public IActionResult Detail(string? id)
    {
        var model = db.Event.Find(id);

        if (model == null)
        {
            return RedirectToAction("Index");
        }

        return View(model);
    }

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


    // GET: Home/Insert
    public IActionResult Insert()
    {
        ViewBag.EventList = new SelectList(db.Event, "Id", "Name");
        return View();
    }

    // POST: Home/Insert
    [HttpPost]
    public IActionResult Insert(EventVM vm)
    {
        if (ModelState.IsValid("Id") &&
            db.Event.Any(e => e.Id == vm.Id))
        {
            ModelState.AddModelError("Id", "Duplicated Id");
        }

        if (ModelState.IsValid("CategoryId") &&
            db.Event.Any(c => c.Id == vm.CategoryId))
        {
            ModelState.AddModelError("CategoryId", "Invalid Id");
        }

        if (ModelState.IsValid("VenueId") &&
            db.Event.Any(v => v.Id == vm.VenueId))
        {
            ModelState.AddModelError("VenueId", "Invalid Id");
        }


        if (ModelState.IsValid)
        {
            db.Event.Add(new()
            {
                Id = vm.Id.Trim().ToUpper(),
                Name = vm.Name.Trim().ToUpper(),
                Description = vm.Description,
                StartDate = vm.StartDate,
                EndDate = vm.EndDate,
                StartTime = vm.StartTime,
                EndTime = vm.EndTime,
                Price = vm.Price,
                PosterImg = vm.PosterImg,
                BannerImg = vm.BannerImg,
                Status = "Active",
                TotalTickets = 0,
                TicketSold = 0,
                CategoryId = vm.CategoryId,
                VenueId = vm.VenueId
            });
            db.SaveChanges();
            TempData["Info"] = "Record inserted.";
            return RedirectToAction("EventRead");
        }

        ViewBag.EventList = new SelectList(db.Category, "Id", "Name");
        return View();
    }
        
    // GET: Home/Update
    public IActionResult Update(string? id)
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
            CategoryId = e.CategoryId,
            VenueId = e.VenueId
        };

        ViewBag.EventList = new SelectList(db.Category, "Id", "Name");
        return View(vm); 
    }

    // POST: Home/Update
    [HttpPost]
    public IActionResult Update(EventVM vm)
    {
        var e = db.Event.Find(vm.Id);

        if (e == null)
        {
            return RedirectToAction("EventRead");
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
            e.PosterImg = vm.PosterImg;
            e.BannerImg = vm.BannerImg;
            e.Status = vm.Status;
            e.TotalTickets = vm.TotalTickets;
            e.TicketSold = vm.TicketSold;
            e.CategoryId = vm.CategoryId;
            e.VenueId = vm.VenueId;
            db.SaveChanges();

            TempData["Info"] = "Record updated.";
            return RedirectToAction("Index");
        }

        ViewBag.ProgramList = new SelectList(db.Category, "Id", "Name");
        return View(vm);
    }

    // POST: Home/Delete
    [HttpPost]
    public IActionResult Delete(string? id)
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
            });
        }

        db.SaveChanges();

        TempData["Info"] = "Record(s) restored.";
        return RedirectToAction("UNKOWNYET");
    }
}
