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

            if (db.Category.Any(c => c.Id == vm.Id))
            {
                ModelState.AddModelError("Id", "Category ID already exists.");
                return View(vm);
            }

            var category = new Category
            {
                Id = vm.Id,
                CategoryName = vm.Name
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
            var categories = db.Category.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                categories = categories.Where(c => c.CategoryName.Contains(search));
            }

            return View(categories.ToList());
        }
    }
}