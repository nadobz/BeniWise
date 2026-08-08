using System.Security.Claims;
using BeniWise.DataModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeniWise.WebApp.Controllers
{
    [Authorize]
    public class FeedbackController : Controller
    {
        private readonly AppDbContext _context;

        public FeedbackController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Feedback/Create/5
        [Authorize]
        public async Task<IActionResult> Create(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // Only allow the customer to give feedback for their own order
            var order = await _context.Orders
                .FirstOrDefaultAsync(o =>
                    o.Id == orderId &&
                    o.UserId == userId);

            if (order == null)
                return NotFound();

            // Feedback can only be submitted after pickup
            if (order.Status != OrderStatus.Completed)
            {
                TempData["FeedbackError"] =
                    "You can only submit feedback after picking up your order.";

                return RedirectToAction("Index", "Orders");
            }

            // Prevent duplicate feedback
            var existingFeedback = await _context.Feedbacks
                .FirstOrDefaultAsync(f =>
                    f.OrderId == orderId &&
                    f.UserId == userId);

            if (existingFeedback != null)
            {
                TempData["FeedbackError"] =
                    "You have already submitted feedback for this order.";

                return RedirectToAction("Index", "Orders");
            }

            ViewBag.OrderId = orderId;

            return View(new Feedback
            {
                OrderId = orderId
            });
        }

        // POST: /Feedback/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Feedback feedback)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // Make sure the order belongs to the logged-in customer
            var order = await _context.Orders
                .FirstOrDefaultAsync(o =>
                    o.Id == feedback.OrderId &&
                    o.UserId == userId);

            if (order == null)
                return NotFound();

            // Feedback is only allowed for completed orders
            if (order.Status != OrderStatus.Completed)
            {
                TempData["FeedbackError"] =
                    "You can only submit feedback after picking up your order.";

                return RedirectToAction("Index", "Orders");
            }

            // Prevent duplicate feedback
            var existingFeedback = await _context.Feedbacks
                .FirstOrDefaultAsync(f =>
                    f.OrderId == feedback.OrderId &&
                    f.UserId == userId);

            if (existingFeedback != null)
            {
                TempData["FeedbackError"] =
                    "You have already submitted feedback for this order.";

                return RedirectToAction("Index", "Orders");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.OrderId = feedback.OrderId;
                return View(feedback);
            }

            feedback.UserId = userId;
            feedback.DateSubmitted = DateTime.Now;

            _context.Feedbacks.Add(feedback);

            await _context.SaveChangesAsync();

            TempData["FeedbackSuccess"] =
                "Thank you! Your feedback has been submitted.";

            return RedirectToAction("Index", "Orders");
        }

        // GET: /Feedback/Manage
        [Authorize(Roles = "Admin,CafeteriaStaff")]
        public async Task<IActionResult> Manage()
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.Order)
                .OrderByDescending(f => f.DateSubmitted)
                .ToListAsync();

            return View(feedbacks);
        }

        // GET: /Feedback/Report
        [Authorize(Roles = "Admin,CafeteriaStaff")]
        public async Task<IActionResult> Report()
        {
            var feedbacks = await _context.Feedbacks
                .ToListAsync();

            ViewBag.TotalFeedback = feedbacks.Count;

            ViewBag.AverageRating = feedbacks.Any()
                ? feedbacks.Average(f => f.Rating)
                : 0;

            ViewBag.FiveStars = feedbacks.Count(f => f.Rating == 5);
            ViewBag.FourStars = feedbacks.Count(f => f.Rating == 4);
            ViewBag.ThreeStars = feedbacks.Count(f => f.Rating == 3);
            ViewBag.TwoStars = feedbacks.Count(f => f.Rating == 2);
            ViewBag.OneStar = feedbacks.Count(f => f.Rating == 1);

            return View();
        }
    }
}