using DEM__Diamond_Event_Management_;
using DEM__Diamond_Event_Management_.Models;
using Microsoft.AspNetCore.Mvc;

namespace DEM__Diamond_Event_Management_.Controllers;

public class IndexBannerController(DB db, Helper hp) : Controller
{
    // 1. View all banners in their own table (not tied to any Event)
    [HttpGet]
    public IActionResult IndexBannerRead(string? name)
    {
        var query = db.IndexBanner.AsQueryable();

        // search by name
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(b => b.Name != null && b.Name.Contains(name));
        }

        var indexBanners = query.Select(e => new IndexBannerVM
        {
            Id = e.Id,
            Name = e.Name,
            IndexBannerImg = e.IndexBannerImg,
        }).ToList();

        return View(indexBanners);
    }

    // 2. Load the empty form to add a brand new banner
    [HttpGet]
    public IActionResult IndexBannerCreate()
    {
        return View();
    }

    // 3. Insert a new banner row + save its image
    [HttpPost]
    public IActionResult IndexBannerCreate(IndexBannerVM vm, IFormFile? indexBannerFile)
    {
        if (ModelState.IsValid("Id") && db.IndexBanner.Any(b => b.Id == vm.Id))
        {
            ModelState.AddModelError("Id", "IndexBanner ID already exists.");
        }

        if (indexBannerFile == null || indexBannerFile.Length == 0)
        {
            ModelState.AddModelError("IndexBannerImg", "Please select an image file.");
        }
        else
        {
            var error = hp.ValidatePhoto(indexBannerFile);
            if (error != "")
            {
                ModelState.AddModelError("IndexBannerImg", error);
            }
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var banner = new IndexBanner
        {
            Id = vm.Id,
            Name = vm.Name,
            IndexBannerImg = hp.SavePhoto(indexBannerFile!, "uploads", 3840, 1200)
        };

        db.IndexBanner.Add(banner);
        db.SaveChanges();

        TempData["Info"] = "Banner successfully created.";
        return RedirectToAction("IndexBannerRead");
    }

    // 4. Load the upload form for an existing banner
    [HttpGet]
    public IActionResult IndexBannerUpdate(string? id)
    {
        var e = db.IndexBanner.Find(id);
        if (e == null) return RedirectToAction("IndexBannerRead");

        var vm = new IndexBannerVM
        {
            Id = e.Id,
            Name = e.Name,
            IndexBannerImg = e.IndexBannerImg,
        };

        return View(vm);
    }

    // 5. Handle the image/name update and save it to the database
    [HttpPost]
    public IActionResult IndexBannerUpdate(IndexBannerVM vm, IFormFile? indexBannerFile)
    {
        var e = db.IndexBanner.Find(vm.Id);
        if (e == null) return RedirectToAction("IndexBannerRead");

        e.Name = vm.Name;

        if (indexBannerFile != null && indexBannerFile.Length > 0)
        {
            var error = hp.ValidatePhoto(indexBannerFile);
            if (error != "")
            {
                ModelState.AddModelError("IndexBannerImg", error);
                vm.IndexBannerImg = e.IndexBannerImg;
                return View(vm);
            }

            if (!string.IsNullOrEmpty(e.IndexBannerImg))
            {
                hp.DeletePhoto(e.IndexBannerImg, "uploads");
            }

            // Save new photo
            e.IndexBannerImg = hp.SavePhoto(indexBannerFile, "uploads", 3840, 1200);
        }

        db.SaveChanges();

        TempData["Info"] = "Banner successfully updated.";
        return RedirectToAction("IndexBannerRead");
    }

    // 6. Remove a banner entirely (row + image file)
    [HttpPost]
    public IActionResult IndexBannerDelete(string? id)
    {
        var e = db.IndexBanner.Find(id);
        if (e != null)
        {
            if (!string.IsNullOrEmpty(e.IndexBannerImg))
            {
                hp.DeletePhoto(e.IndexBannerImg, "uploads");
            }

            db.IndexBanner.Remove(e);
            db.SaveChanges();

            TempData["Info"] = "Banner deleted.";
        }

        return RedirectToAction("IndexBannerRead");
    }
}
