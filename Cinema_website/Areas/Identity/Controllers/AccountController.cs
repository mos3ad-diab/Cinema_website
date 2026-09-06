using Cinema_website.Models;
using Cinema_website.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Cinema_website.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if(!ModelState.IsValid)
            {
                return View(registerVM);
            }
            var user = new ApplicationUser()
            {
                Name = registerVM.Name,
                UserName = registerVM.UserName,
                Address = registerVM.Address,
                Email = registerVM.Email
            };

            var result = await _userManager.CreateAsync(user, registerVM.Password);

            if (!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(registerVM);
            }

            var link = Url.Action();
            
            await _emailSender.SendEmailAsync(
                registerVM.Email,
                "Falcon Cinema Confirmation",
                $"<h1> please click <a href = {link}> here </a> to confirm your account </h1>"
                );

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            var user = await _userManager.FindByEmailAsync(loginVM.UserNameOrEmail) ??
                await _userManager.FindByNameAsync(loginVM.UserNameOrEmail);

            if(user is null)
            {
                ModelState.AddModelError("", "Invalid user name or password");
                return View(loginVM);
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, true);

            if (!result.Succeeded)
            {
                if(result.IsLockedOut)
                {
                    ModelState.AddModelError("", "to many attemps please try again later");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid user name or password");
                }
                return View(loginVM);

            }

            return RedirectToAction("Index", "Cinema", new { area = "Admin" });
        }
    }
}
