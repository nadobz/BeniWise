using BeniWise.DataModel;
using BeniWise.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BeniWise.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context) => _context = context;

        public async Task<IActionResult> Index(int? categoryId)
        {
            var hour = DateTime.Now.Hour;
            ViewBag.Greeting = hour switch
            {
                < 12 => "Good Morning!",
                < 18 => "Good Afternoon!",
                _ => "Good Evening!"
            };

            var mealsQuery = _context.MenuItems.Include(m => m.Category).AsQueryable();

            if (categoryId.HasValue)
                mealsQuery = mealsQuery.Where(m => m.CategoryId == categoryId);

            ViewBag.FeaturedMeals = await mealsQuery
                .OrderByDescending(m => m.Id)
                .Take(6)
                .ToListAsync();

            ViewBag.SelectedCategoryId = categoryId;

            ViewBag.QuickCategories = await _context.Categories
                .Include(c => c.MenuItems)
                .Take(3)
                .ToListAsync();

            return View();
        }

        public IActionResult Privacy() => View();

        public IActionResult About() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            return View();
        }

        [Authorize(Roles = "Student")]
        public IActionResult StudentDashboard() => View();

        [Authorize(Roles = "CafeteriaStaff")]
        public IActionResult StaffDashboard() => View();

    }
}