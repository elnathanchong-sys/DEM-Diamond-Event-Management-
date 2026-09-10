using DEM__Diamond_Event_Management_.Models;
using Microsoft.AspNetCore.Mvc;

namespace DEM__Diamond_Event_Management_.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DB db;

        public CategoryController(DB db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult CatCreate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CatCreate(CategoryVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            // Find the latest category ID
            var lastCategory = db.Category
                .OrderByDescending(c => c.Id)
                .FirstOrDefault();

            int nextNumber = 1;

            if (lastCategory != null)
            {
                nextNumber = int.Parse(lastCategory.Id.Substring(1)) + 1;
            }

            // Generate new Category ID
            string newId = "C" + nextNumber.ToString("D3");

            // Create category
            var category = new Category
            {
                Id = newId,
                CategoryName = vm.Name,
                Status = "Visible"
            };

            db.Category.Add(category);
            db.SaveChanges();

            return RedirectToAction("CatRead");
        }

        [HttpGet]
        public IActionResult CatUpdate(string id)
        {
            var category = db.Category.Find(id);

            if (category == null)
            {
                return RedirectToAction("CatRead");
            }

            var vm = new CategoryVM
            {
                Id = category.Id,
                Name = category.CategoryName,
                Description = ""
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult CatUpdate(CategoryVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var category = db.Category.Find(vm.Id);

            if (category == null)
            {
                return RedirectToAction("CatRead");
            }

            category.CategoryName = vm.Name;

            db.SaveChanges();

            return RedirectToAction("CatRead");
        }

        public IActionResult CatRead(string search)
        {
            var categories = db.Category
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                categories = categories.Where(c => c.CategoryName.Contains(search));
            }

            return View(categories.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CatDelete(string id)
        {
            var category = db.Category.Find(id);

            if (category == null)
            {
                return RedirectToAction("CatRead");
            }

            category.Status = "Hidden";
            db.SaveChanges();

            return RedirectToAction("CatRead");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CatUnhide(string id)
        {
            var category = db.Category.Find(id);

            if (category == null)
            {
                return RedirectToAction("CatRead");
            }

            category.Status = "Visible";
            db.SaveChanges();

            return RedirectToAction("CatRead");
        }
    }
}