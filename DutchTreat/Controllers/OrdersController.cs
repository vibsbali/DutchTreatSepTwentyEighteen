using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DutchTreat.Controllers
{
   [Route("api/[Controller]")]
   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
   public class OrdersController : Controller
   {
      private IDutchRepository dutchRepository;
      private ILogger<OrdersController> logger;
      private IMapper mapper;
      private readonly UserManager<StoreUser> userManager;

      public OrdersController(IDutchRepository dutchRepository, 
         ILogger<OrdersController> logger, 
         IMapper mapper,
         UserManager<StoreUser> userManager)
      {
         this.dutchRepository = dutchRepository;
         this.logger = logger;
         this.mapper = mapper;
         this.userManager = userManager;
      }

      [HttpGet]
      public IActionResult GetOrders(bool includeItems = true)
      {
         try
         {
            var username = User.Identity.Name;
            var results = dutchRepository.GetAllOrdersByUser(username, includeItems);
            return Ok(mapper.Map<IEnumerable<Order>, IEnumerable<OrderViewModel>>(results));
         }
         catch (Exception e)
         {
            logger.LogError($"Error occured while getting {e}");
            return BadRequest();
         }
      }

      [HttpGet("{id:int}")]
      public IActionResult Get(int id)
      {
         try
         {
            var username = User.Identity.Name;
            var order = dutchRepository.GetOrderById(username, id);

            if (order != null)
            {
               var orderViewModel = mapper.Map<Order, OrderViewModel>(order);
               return Ok(orderViewModel);
            }
            return NotFound();
         }
         catch (Exception e)
         {
            logger.LogError($"Error occured while getting {e}");
            return BadRequest();
         }
      }

      [HttpPost]
      public async Task<IActionResult> Post([FromBody]OrderViewModel model)
      {
         try
         {
            if (ModelState.IsValid)
            {
               var newOrder = mapper.Map<OrderViewModel, Order>(model);

               if (newOrder.OrderDate == DateTime.MinValue)
               {
                  newOrder.OrderDate = DateTime.Now;
               }

               newOrder.User = await userManager.FindByNameAsync(User.Identity.Name);
               dutchRepository.AddEntity(newOrder);

               if (dutchRepository.SaveAll())
               {
                  var vm = mapper.Map<Order, OrderViewModel>(newOrder);

                  return Created($"/api/orders/{vm.OrderId}", model);
               }
            }
            else
            {
               return BadRequest(ModelState);
            }
         }
         catch (Exception e)
         {
            logger.LogError($"Failed to save {e}");
         }

         return BadRequest("Failed to Save");
      }
   }
}
