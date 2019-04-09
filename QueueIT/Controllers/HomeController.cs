using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using QueueIT.Identity;
using QueueIT.InputModels;
using QueueIT.Models;
using QueueIT.ViewModels;
using ForgotPasswordModel = QueueIT.Models.ForgotPasswordModel;
using RegisterModel = QueueIT.Models.RegisterModel;
using ResetPasswordModel = QueueIT.Models.ResetPasswordModel;

namespace QueueIT.Controllers
{
    public class HomeController : Controller
    {
//        private readonly QueueItDbContext _db;
        private readonly UserManager<QueueItUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<QueueItUser> _claimsPrincipalFactory;
        private readonly QueueItDbContext _db;

        // Constructor
        public HomeController(UserManager<QueueItUser> userManager, 
            IUserClaimsPrincipalFactory<QueueItUser> claimsPrincipalFactory,
            QueueItDbContext queueItDbContext)
        {
            _userManager = userManager;
            _claimsPrincipalFactory = claimsPrincipalFactory;
            _db = queueItDbContext;
        }

        // Landing Page
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.LoginUserName);

             
                if (user != null && await _userManager.CheckPasswordAsync(user, model.LoginPassword))
                {
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError("login-err", "Email is not confirmed");
                        return View("Index");
                    }
                    
                    var principal = await _claimsPrincipalFactory.CreateAsync(user);

                    await HttpContext.SignInAsync("Identity.Application", principal);

                    return RedirectToAction("UserHome", "Account");
                }

                ModelState.AddModelError("login-err", "Invalid UserName or Password");
            }
            
            ModelState.AddModelError("login-err", "");
            return View("Index");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync("Identity.Application");
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                
                if (user == null)
                {
                    user = new QueueItUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        UserName = model.UserName,
                        Email = model.Email
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);
                    
                    if (result.Succeeded)
                    {
                        var team = new Team
                        {
                            Name = "Personal", 
                            CreatorId = user.Id,
                            Description = "This is your Personal Team. " +
                                          "Use it to keep track of your Queues and Tasks " +
                                          "that you do not want to share with your team.",
                            IsPrivate = true,
                            CreatedOn = DateTime.Now
                        };
                        _db.Teams.Add(team);
                        _db.SaveChanges();
                        
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationEmail = Url.Action("ConfirmEmailAddress", "Home",
                            new {token = token, email = user.Email }, Request.Scheme);
                        System.IO.File.WriteAllText("confirmationLink.txt", confirmationEmail); 
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("reg-err", error.Description);
                        }

                        return View("Index");
                    }
                }

                return View("Success");
            }
            
            ModelState.AddModelError("reg-err", "");
            return View("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmailAddress(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    return View("Success");
                }
            }

            return View("Error");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetUrl = Url.Action("ResetPassword", "Home",
                        new {token = token, email = user.Email}, Request.Scheme);
                    
                    System.IO.File.WriteAllText("resetLink.txt", resetUrl);  
                }
                else
                {
                    Console.WriteLine("Email Not found");
                    // email user and inform them that they do not have an account
                }

                return View("Success");
            }
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            return View(new ResetPasswordModel {Token = token, Email = email});
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }

                        return View();
                    }

                    return View("Success");
                }
                
                ModelState.AddModelError("", "Invalid Request");
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}