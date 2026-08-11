using BeniWise.DataModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BeniWise.WebApp.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Orders
        // Customer's own orders
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var feedbackOrderIds = await _context.Feedbacks
                .Where(f => f.UserId == userId)
                .Select(f => f.OrderId)
                .ToListAsync();

            ViewBag.FeedbackOrderIds = feedbackOrderIds;

            return View(orders);
        }

        // GET: /Orders/Confirmation/5
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Confirmation(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o =>
                    o.Id == id &&
                    o.UserId == userId);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // GET: /Orders/Manage
        [Authorize(Roles = "Admin,CafeteriaStaff")]
        public async Task<IActionResult> Manage()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            // Load the ApplicationUser for each order
            foreach (var order in orders)
            {
                order.User = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == order.UserId);
            }

            return View(orders);
        }

        // GET: /Orders/ManageDetails/5
        [Authorize(Roles = "Admin,CafeteriaStaff")]
        public async Task<IActionResult> ManageDetails(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            order.User = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == order.UserId);

            return View(order);
        }

        // POST: /Orders/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,CafeteriaStaff")]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            // Only allow valid staff status transitions
            bool validTransition =
                (order.Status == OrderStatus.Pending &&
                 status == OrderStatus.Preparing)

                ||

                (order.Status == OrderStatus.Preparing &&
                 status == OrderStatus.ReadyForPickup);

            if (!validTransition)
            {
                TempData["OrderError"] =
                    "Invalid status change. Orders must follow the correct process.";

                return RedirectToAction(nameof(Manage));
            }

            order.Status = status;

            await _context.SaveChangesAsync();

            TempData["OrderSuccess"] =
                "Order status updated successfully.";

            return RedirectToAction(nameof(Manage));
        }

        // POST: /Orders/ConfirmPickup
        // Customer confirms that they have picked up the order
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> ConfirmPickup(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // Only find the order if it belongs to the logged-in customer
            var order = await _context.Orders
                .FirstOrDefaultAsync(o =>
                    o.Id == id &&
                    o.UserId == userId);

            if (order == null)
                return NotFound();

            // Customer can only confirm pickup when the order is ready
            if (order.Status != OrderStatus.ReadyForPickup)
            {
                TempData["OrderError"] =
                    "This order is not ready for pickup yet.";

                return RedirectToAction(nameof(Index));
            }

            // Customer has picked up the order
            order.Status = OrderStatus.Completed;

            await _context.SaveChangesAsync();

            TempData["OrderSuccess"] =
                "Order pickup confirmed successfully!";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Orders/BestSellers
        [Authorize(Roles = "Admin,CafeteriaStaff")]
        public async Task<IActionResult> BestSellers()
        {
            var bestSellers = await _context.OrderItems
                .Include(oi => oi.MenuItem)
                .Include(oi => oi.Order)
                .Where(oi => oi.Order != null &&
                             oi.Order.Status == OrderStatus.Completed)
                .GroupBy(oi => new
                {
                    oi.MenuItemId,
                    MenuItemName = oi.MenuItem!.Name
                })
                .Select(g => new
                {
                    MenuItemName = g.Key.MenuItemName,
                    QuantitySold = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(x => x.QuantitySold)
                .ToListAsync();

            return View(bestSellers);
        }
    }
}