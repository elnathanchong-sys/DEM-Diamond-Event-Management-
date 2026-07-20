using DEM__Diamond_Event_Management_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DEM.Controllers;

public class EventController(DB db, 
                            IWebHostEnvironment en) : Controller
{
    // GET: Home/Index
    public IActionResult Index()
    {
        var model = db.Event;
        return View(model);
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

    // GET: Home/CheckId 
    public bool CheckId(string id)
    {
        return !db.Event.Any(s => s.Id == id);
    }

    // GET: Home/CheckProgramId
    public bool CheckProgramId(string programId)
    {
        return db.Programs.Any(p => p.Id == programId);
    }

    // GET: Home/Insert
    public IActionResult Insert()
    {
        ViewBag.ProgramList = new SelectList(db.Programs, "Id", "Name");
        return View();
    }

    // POST: Home/Insert
    [HttpPost]
    public IActionResult Insert(EventVM vm)
    {
        if (ModelState.IsValid("Id") &&
            db.Event.Any(s => s.Id == vm.Id))
        {
            ModelState.AddModelError("Id", "Duplicated Id");
        }

        if (ModelState.IsValid("ProgramId") &&
            db.Event.Any(s => s.Id == vm.Id))
        {
            ModelState.AddModelError("ProgramId", "Invalid Id");
        }

        if (ModelState.IsValid)
        {
            db.Event.Add(new()
            {
                Id = vm.Id.Trim().ToUpper(),
                Name = vm.Name.Trim().ToUpper(),
                Gender = vm.Gender,
                ProgramId = vm.ProgramId,
            });
            db.SaveChanges();
            TempData["Info"] = "Record inserted.";
            return RedirectToAction("Index");
        }

        ViewBag.ProgramList = new SelectList(db.Programs, "Id", "Name");
        return View();
    }
        
    // GET: Home/Update
    public IActionResult Update(string? id)
    {
        var s = db.Event.Find(id);

        if (s == null)
        {
            return RedirectToAction("Index");
        }

        var vm = new EventVM
        {
            Id = s.Id,
            Name = s.Name,
            Gender = s.Gender,
            ProgramId = s.ProgramId,
        };

        ViewBag.ProgramList = new SelectList(db.Programs, "Id", "Name");
        return View(vm); // TODO
    }

    // POST: Home/Update
    [HttpPost]
    public IActionResult Update(EventVM vm)
    {
        var s = db.Event.Find(vm.Id);

        if (s == null)
        {
            return RedirectToAction("Index");
        }

        if (ModelState.IsValid("ProgramId") &&
            !db.Programs.Any(p => p.Id == vm.ProgramId))
        {
            ModelState.AddModelError("ProgramId", "Invalid Program.");
        }

        if (ModelState.IsValid)
        {
            s.Name = vm.Name.Trim().ToUpper();
            s.Gender = vm.Gender;
            s.ProgramId = vm.ProgramId;
            db.SaveChanges();

            TempData["Info"] = "Record updated.";
            return RedirectToAction("Index");
        }

        ViewBag.ProgramList = new SelectList(db.Programs, "Id", "Name");
        return View(vm);
    }

    // POST: Home/Delete
    [HttpPost]
    public IActionResult Delete(string? id)
    {
        var s = db.Event.Find(id);

        if (s != null)
        {
            db.Event.Remove(s);
            db.SaveChanges();

            TempData["Info"] = "Record deleted.";
        }

        return Redirect(Request.Headers.Referer.ToString());
    }

    // GET: Home/Demo
    public IActionResult Demo()
    {
        var model = db.Event;
        return View(model);
    }

    // POST: Home/DeleteMany
    [HttpPost]
    public IActionResult DeleteMany(string[] ids)
    {
        int n = db.Event
                  .Where(s => ids.Contains(s.Id))
                  .ExecuteDelete();

        TempData["Info"] = $"{n} record(s) deleted.";
        return RedirectToAction("Demo");
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
                Gender = data[2],
                ProgramId = data[3],
            });
        }

        db.SaveChanges();

        TempData["Info"] = "Record(s) restored.";
        return RedirectToAction("Demo");
    }
}
