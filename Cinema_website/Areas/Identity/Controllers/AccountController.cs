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
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var link = Url.Action("ConfirmEmail" , "Account", new {area = "Identity", userId = user.Id , token = token} , Request.Scheme);
            
            await _emailSender.SendEmailAsync(
                registerVM.Email,
                "Falcon Cinema Confirmation",
                $"<h1> please click <a href = {link}> here </a> to confirm your account </h1>"
                );

            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if(!result.Succeeded)
            {
                return RedirectToAction(nameof(Login));
            }
            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(EmailConfirmationVM emailConfirmationVM)
        {
            var user = await _userManager.FindByEmailAsync(emailConfirmationVM.UserNameOrEmail) ??
               await _userManager.FindByNameAsync(emailConfirmationVM.UserNameOrEmail);


            if (user is null)
            {
                ModelState.AddModelError("", "User not found");
                return View(emailConfirmationVM);
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "This email is already confirmed.");
                return View(emailConfirmationVM);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var link = Url.Action("ConfirmEmail", "Account", new { area = "Identity", userId = user.Id, token = token }, Request.Scheme);

            await _emailSender.SendEmailAsync(
                user.Email,
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
                else if(result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "please confirme your email");
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
