using System;
using System.Collections.Generic;
using System.Text;
using BeniWise.DataModel;
using BeniWise.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BeniWise.WebApp.Controllers
{
    // Only Admins can create/manage Staff accounts, per the "staff accounts
    // are added by admin accounts" requirement — staff never self-register.
    [Authorize(Roles = "Admin")]
    public class AccountsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountsController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: /Accounts/Staff  (list of current staff accounts)
        public async Task<IActionResult> Staff()
        {
            var staffUsers = await _userManager.GetUsersInRoleAsync("CafeteriaStaff");
            return View(staffUsers);
        }

        // GET: /Accounts/CreateStaff
        public IActionResult CreateStaff()
        {
            return View(new CreateStaffViewModel());
        }

        // POST: /Accounts/CreateStaff
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStaff(CreateStaffViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = new ApplicationUser
            {
                UserName = vm.Email,
                Email = vm.Email,
                Firstname = vm.Firstname,
                Lastname = vm.Lastname
            };

            var result = await _userManager.CreateAsync(user, vm.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "CafeteriaStaff");
                TempData["Success"] = $"Staff account created for {vm.Email}.";
                return RedirectToAction(nameof(Staff));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(vm);
        }

        // POST: /Accounts/RemoveStaff/{id}
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveStaff(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.RemoveFromRoleAsync(user, "CafeteriaStaff");
            await _userManager.DeleteAsync(user);

            TempData["Success"] = "Staff account removed.";
            return RedirectToAction(nameof(Staff));
        }
    }
}