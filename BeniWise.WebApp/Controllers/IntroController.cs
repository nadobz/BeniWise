using BeniWise.DataModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BeniWise.WebApp.Controllers
{
    public class IntroController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IntroController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: /Intro/Index  (this is the login page)
        public IActionResult Index()
        {
            return View();
        }

        // POST: /Intro/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Email and password are required.");
                return View("Index");
            }

            var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(email);

                // Staff go straight into the Admin Panel (Menu Management) — that's
                // the real page your classmate built, not the empty StaffDashboard.
                if (user != null && await _userManager.IsInRoleAsync(user, "CafeteriaStaff"))
                    return RedirectToAction("Index", "MenuItems");

                if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
                    return RedirectToAction("Index", "MenuItems");

                // Students land on the real dashboard (Home/Index) with featured meals data.
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid email or password.");
            return View("Index");
        }

        // POST: /Intro/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }

        // GET: /Intro/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Intro/Register
        [HttpPost]
        public async Task<IActionResult> Register(string email, string password, string fullName)
        {
            // Your ApplicationUser has FirstName/LastName, not a single FullName field.
            // The form only sends one "fullName" text box, so split it here.
            var nameParts = (fullName ?? "").Trim().Split(' ', 2);
            var firstName = nameParts.Length > 0 ? nameParts[0] : "";
            var lastName = nameParts.Length > 1 ? nameParts[1] : "";

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                Firstname = firstName,
                Lastname = lastName
            };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // Auto-assign Student role (role is seeded on startup in Program.cs)
                await _userManager.AddToRoleAsync(user, "Student");

                // Sign the user in immediately after registering
                await _signInManager.SignInAsync(user, isPersistent: false);

                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View();
        }
    }
}