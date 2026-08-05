using BeniWise.DataModel;
using BeniWise.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeniWise.WebApp.Controllers
{
    public class MenuItemsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public MenuItemsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /MenuItems?categoryId=2&search=chicken
        public async Task<IActionResult> Index(int? categoryId, string? search)
        {
            var query = _context.MenuItems.Include(m => m.Category).AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(m => m.CategoryId == categoryId);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(m => m.Name.Contains(search));

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.Search = search;

            return View(await query.ToListAsync());
        }

        // GET: /MenuItems/Details/5  (the "food information" page)
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.MenuItems.Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /MenuItems/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(new MenuItemFormViewModel());
        }

        // POST: /MenuItems/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MenuItemFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
                return View(vm);
            }

            var item = new MenuItem
            {
                Name = vm.Name,
                Description = vm.Description,
                Price = vm.Price,
                Calories = vm.Calories,
                Ingredients = vm.Ingredients,
                Allergens = vm.Allergens,
                CategoryId = vm.CategoryId
            };

            item.ImagePath = await SaveImageAsync(vm.ImageFile);

            _context.MenuItems.Add(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /MenuItems/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.MenuItems.FindAsync(id);
            if (item == null) return NotFound();

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(new MenuItemFormViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                Calories = item.Calories,
                Ingredients = item.Ingredients,
                Allergens = item.Allergens,
                CategoryId = item.CategoryId,
                ExistingImagePath = item.ImagePath
            });
        }

        // POST: /MenuItems/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MenuItemFormViewModel vm)
        {
            var item = await _context.MenuItems.FindAsync(id);
            if (item == null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
                return View(vm);
            }

            item.Name = vm.Name;
            item.Description = vm.Description;
            item.Price = vm.Price;
            item.Calories = vm.Calories;
            item.Ingredients = vm.Ingredients;
            item.Allergens = vm.Allergens;
            item.CategoryId = vm.CategoryId;

            if (vm.ImageFile != null)
                item.ImagePath = await SaveImageAsync(vm.ImageFile);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: /MenuItems/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.MenuItems.FindAsync(id);
            if (item == null) return NotFound();

            _context.MenuItems.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<string?> SaveImageAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "menu");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/menu/{fileName}";
        }
    }
}
