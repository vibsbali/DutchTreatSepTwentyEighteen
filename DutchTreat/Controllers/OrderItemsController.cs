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
   [Route("/api/orders/{orderid}/items")]
    public class OrderItemsController : Controller
    {
       private readonly IDutchRepository dutchRepository;
       private readonly ILogger<OrderItemsController> logger;
       private readonly IMapper mapper;

       public OrderItemsController(IDutchRepository dutchRepository, ILogger<OrderItemsController> logger, IMapper mapper)
       {
          this.dutchRepository = dutchRepository;
          this.logger = logger;
          this.mapper = mapper;
       }

       [HttpGet]
       public IActionResult Get(int orderId)
       {
          var order = dutchRepository.GetOrderById(orderId);
          if (order != null)
          {
             return Ok(mapper.Map<IEnumerable<OrderItem>, IEnumerable<OrderItemViewModel>>(order.Items));
          }

          return NotFound();
       }

       [HttpGet("{id}")]
       public IActionResult Get(int orderId, int id)
       {
          var order = dutchRepository.GetOrderById(orderId);
          if (order != null)
          {
             var item = order.Items.Where(i => i.Id == id).FirstOrDefault();
             if (item != null)
             {
                return Ok(mapper.Map<OrderItem, OrderItemViewModel>(item));
             }
          }
          return NotFound();

       }
   }
}
