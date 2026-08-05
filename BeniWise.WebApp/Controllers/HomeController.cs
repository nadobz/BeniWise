using BeniWise.DataModel;
using BeniWise.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BeniWise.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var hour = DateTime.Now.Hour;
            ViewBag.Greeting = hour switch
            {
                < 12 => "Good Morning!",
                < 18 => "Good Afternoon!",
                _ => "Good Evening!"
            };

            ViewBag.FeaturedMeals = await _context.MenuItems
                .Include(m => m.Category)
                .OrderByDescending(m => m.Id)
                .Take(3)
                .ToListAsync();

            ViewBag.QuickCategories = await _context.Categories
                .Include(c => c.MenuItems)
                .Take(3)
                .ToListAsync();

            return View();
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}