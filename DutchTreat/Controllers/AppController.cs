using System;
using System.Linq;
using DutchTreat.Data;
using DutchTreat.Services;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DutchTreat.Controllers
{
   public class AppController : Controller
   {
      private readonly IMailService _mailService;
      private readonly IDutchRepository _dutchRepository;

      public AppController(IMailService mailService, IDutchRepository dutchRepository)
      {
         this._mailService = mailService;
         this._dutchRepository = dutchRepository;
      }
      public IActionResult Index()
      {
         return View();
      }

      [HttpGet("contact")]
      public IActionResult Contact()
      {
         return View();
      }

      [HttpPost("contact")]
      public IActionResult Contact(ContactViewModel model)
      {
         if (ModelState.IsValid)
         {
            //send mail
            _mailService.SendMessage("vbali@internode.on.net", model.Subject, $"{model.Name} - {model.Message}");
            ModelState.Clear();
            ViewBag.UserMessage = "Mail Sent";
         }

         return View();

      }

      public IActionResult About()
      {
         ViewBag.Title = "About Us";
         return View();
      }

      public IActionResult Shop()
      {
         var results = _dutchRepository.GetAllProducts();

         return View(results);
      }
   }
}
