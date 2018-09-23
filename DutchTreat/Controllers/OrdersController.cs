using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DutchTreat.Controllers
{
   [Route("api/[Controller]")]
   public class OrdersController : Controller
   {
      private IDutchRepository dutchRepository;
      private ILogger<OrdersController> logger;
      private IMapper mapper;

      public OrdersController(IDutchRepository dutchRepository, ILogger<OrdersController> logger, IMapper mapper)
      {
         this.dutchRepository = dutchRepository;
         this.logger = logger;
         this.mapper = mapper;
      }

      [HttpGet]
      public IActionResult GetOrders(bool includeItems = true)
      {
         try
         {
            var results = dutchRepository.GetAllOrders(includeItems);
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
            var order = dutchRepository.GetOrderById(id);

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
      public IActionResult Post([FromBody]OrderViewModel model)
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
