using Cinema_website.Models;
using Cinema_website.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Cinema_website.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class ProfileController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) { return NotFound(); }
            var profile = new ProfileVM()
            {
                Name = user.Name,
                Email = user.Email,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber
            };
            return View(profile);
        }

        public async Task<IActionResult> Update(ProfileVM profileVM)
        {
            var user = await _userManager.GetUserAsync(User);

            user.Name = profileVM.Name;
            user.PhoneNumber = profileVM.PhoneNumber;
            user.Address = profileVM.Address;

            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM profileVM)
        {
            if (!ModelState.IsValid)
            {
                return View(profileVM);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(user, profileVM.CurrentPassword, profileVM.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(" ,", result.Errors.Select(e => e.Description));
                TempData["Error_Notification"] = errors;
                return View(profileVM);
            }

            TempData["SuccessMessage"] = "Password changed successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}