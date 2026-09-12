using Cinema_website.Models;
using Cinema_website.Repositories;
using Cinema_website.ViewModels;
using Cinema_website.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Cinema_website.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<ApplicationUserOTP> _applicationUserOTPrepository;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, IRepository<ApplicationUserOTP> applicationUserOTPrepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _applicationUserOTPrepository = applicationUserOTPrepository;
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
            await _userManager.AddToRoleAsync(user, CD.CUSTOMER_ROLE);
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
                
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPasswordAsync(ForgetPasswordVM forgetPasswordVM)
        {
            var user = await _userManager.FindByEmailAsync(forgetPasswordVM.UserNameOrEmail) ??
                await _userManager.FindByNameAsync(forgetPasswordVM.UserNameOrEmail);
            if (user is null)
            {
                ModelState.AddModelError("", "Invalid user name or Email");
                return View(forgetPasswordVM);
            }

            
            var otp = new Random().Next(1000, 9999).ToString();
            var applicationUserOTP = new ApplicationUserOTP( otp , user.Id);
            await _applicationUserOTPrepository.InsertAsync(applicationUserOTP);
            await _applicationUserOTPrepository.CommitAsync();
            await _emailSender.SendEmailAsync(
                user.Email,
                "Falcon Cinema Password Reset",
                $"Use this OTP {otp} to continue the prosess"
                );

            return RedirectToAction(nameof(ConfirmOTP), new { userId = user.Id});
        }

        [HttpGet]
        public IActionResult ConfirmOTP(string userId)
        {
            return View( new ConfirmOTPVM { UserId = userId});
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmOTP(ConfirmOTPVM confirmOTPVM)
        {
            var user = await _userManager.FindByIdAsync(confirmOTPVM.UserId);

            if(user is null)
            {
                ModelState.AddModelError("", "Invalid user");
                return View(confirmOTPVM);
            }
            var otps = await _applicationUserOTPrepository.GetAllAsync(o =>
            o.ApplicationUserId == user.Id &&
            o.IsValid == true &&
            o.ValidTo >= DateTime.UtcNow
            );
            var applicationUserOtp =  otps.OrderByDescending(e => e.CreatedAt).FirstOrDefault();
            if(applicationUserOtp == null || applicationUserOtp.OTP != confirmOTPVM.OTP)
            {
                ModelState.AddModelError("", "Invalid / expired otp");
                return View(confirmOTPVM);
            }
            applicationUserOtp.IsValid = false;
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _applicationUserOTPrepository.CommitAsync();
            return RedirectToAction(nameof(ResetPassword) , new { userId = user.Id , token});
        }

        [HttpGet]
        public IActionResult ResetPassword(string userId , string token)
        {
            return View(new ResetPasswordVM { UserId = userId , Token = token });
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            if(!ModelState.IsValid)
            {
                return View(resetPasswordVM);
            }
            var user = await _userManager.FindByIdAsync(resetPasswordVM.UserId);
            
            if(user is null)
            {
                ModelState.AddModelError("", "Invalid user");
                return View(resetPasswordVM);
            }
            await _userManager.ResetPasswordAsync(user, resetPasswordVM.Token, resetPasswordVM.Password);

            return RedirectToAction(nameof(Login));
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
