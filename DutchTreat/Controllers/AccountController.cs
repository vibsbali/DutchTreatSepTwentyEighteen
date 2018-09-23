using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace DutchTreat.Controllers
{
    public class AccountController : Controller
    {
       private ILogger<AccountController> _logger;
      private readonly SignInManager<StoreUser> signInManager;
       private readonly UserManager<StoreUser> userManager;
       private readonly IConfiguration configuration;

       public AccountController(ILogger<AccountController> logger,
          SignInManager<StoreUser> signInManager,
         UserManager<StoreUser> userManager,
          IConfiguration configuration)
       {
          _logger = logger;
         this.signInManager = signInManager;
          this.userManager = userManager;
          this.configuration = configuration;
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

       [HttpPost]
       public async Task<IActionResult> CreateToken([FromBody] LoginViewModel viewModel)
       {
          if (ModelState.IsValid)
          {
             var user = await userManager.FindByNameAsync(viewModel.UserName);
             if (user != null)
             {

                var result = await signInManager.CheckPasswordSignInAsync(user, viewModel.Password, false);
                if (result.Succeeded)
                {
                   //Create token
                   var claims = new[]
                   {
                      new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                      new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
                   };

                   var stringToken = configuration["Tokens:Key"];
                   var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(stringToken));
                   var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                   var token = new JwtSecurityToken(
                      configuration["Tokens:Issuer"],
                      configuration["Tokens:Audience"],
                      claims,
                      expires:DateTime.UtcNow.AddMinutes(20),
                      signingCredentials:creds
                      );

                   var results = new
                   {
                      token = new JwtSecurityTokenHandler().WriteToken(token),
                      expiration = token.ValidTo
                   };
                   return Created("", results);
                }
             }
          }

          return BadRequest();
       }
    }
}
