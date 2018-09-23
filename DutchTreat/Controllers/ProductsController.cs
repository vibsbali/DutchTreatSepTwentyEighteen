using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DutchTreat.Controllers
{
   [Route("api/[Controller]")]
   [ApiController]
   [Produces("application/json")]
    public class ProductsController : Controller
    {
       private readonly IDutchRepository dutchRepository;
       private readonly ILogger<ProductsController> logger;

       public ProductsController(IDutchRepository dutchRepository, ILogger<ProductsController> logger)
       {
          this.dutchRepository = dutchRepository;
          this.logger = logger;
       }

       [HttpGet]
       [ProducesResponseType(200)]
       [ProducesResponseType(400)]
       public ActionResult<IEnumerable<Product>> GetProducts()
       {
          try
          {
             return Ok(dutchRepository.GetAllProducts());
         }
          catch (Exception e)
          {
             logger.LogError($"An error has occured {e}");
             return BadRequest();
          }
       }
    }
}
