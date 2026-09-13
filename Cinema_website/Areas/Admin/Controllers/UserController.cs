using Cinema_website.Models;
using Cinema_website.Repositories;
using Cinema_website.Utilities;
using Cinema_website.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Cinema_website.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{CD.ADMIN_ROLE} ,{CD.SUPER_ADMIN_ROLE}")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userListVM = new List<UserVM>();

            foreach (var user in users)
            {
                
                var roles = await _userManager.GetRolesAsync(user);

                userListVM.Add(new UserVM
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Address = user.Address,
                    PhoneNumber = user.PhoneNumber,
                    Role = roles.FirstOrDefault() ?? "No Role",
                    LockOutEnd = user.LockoutEnd
                });
            }

            return View(userListVM);
        }
    }
}
