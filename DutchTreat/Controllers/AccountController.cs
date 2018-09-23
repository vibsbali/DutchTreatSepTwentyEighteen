using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DutchTreat.Controllers
{
    public class AccountController : Controller
    {
       private ILogger<AccountController> _logger;
      private readonly SignInManager<StoreUser> signInManager;

      public AccountController(ILogger<AccountController> logger,
          SignInManager<StoreUser> signInManager)
       {
          _logger = logger;
         this.signInManager = signInManager;
      }

       public IActionResult Login()
       {
          if (this.User.Identity.IsAuthenticated)
          {
             return RedirectToAction("Index", "App");
          }

          return View();
       }

       [HttpPost]
       public async Task<IActionResult> Login(LoginViewModel model)
       {
          if (ModelState.IsValid)
          {
             var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);

             if (result.Succeeded)
             {
                if (Request.Query.Keys.Contains("ReturnUrl"))
                {
                   return Redirect(Request.Query["ReturnUrl"].First());
                }

                return RedirectToAction("Shop", "App");
             }
          }

          ModelState.AddModelError("", "Failed to Login");
          return View(model);
       }

       [HttpGet]
       public async Task<IActionResult> Logout()
       {
          await signInManager.SignOutAsync();
          return RedirectToAction("Index", "App");
       }
    }
}
