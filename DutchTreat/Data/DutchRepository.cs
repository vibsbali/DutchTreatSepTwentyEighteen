using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DutchTreat.Data.Entities;
using Microsoft.Extensions.Logging;

namespace DutchTreat.Data
{
   public class DutchRepository : IDutchRepository
   {
      private readonly DutchContext dutchContext;
      private readonly ILogger<DutchRepository> logger;

      public DutchRepository(DutchContext dutchContext, ILogger<DutchRepository> logger)
      {
         this.dutchContext = dutchContext;
         this.logger = logger;
      }

      public IEnumerable<Product> GetAllProducts()
      {
         try
         {
            logger.LogInformation("GetAllProducts was called");
            return dutchContext.Products.OrderBy(p => p.Title).ToList();
         }
         catch (Exception e)
         {
            logger.LogError($"Failed to get all products : {e}");
         }

         return null;
      }

      public IEnumerable<Product> GetProductsByCategory(string category)
      {
         return dutchContext.Products.Where(p => p.Category == category).ToList();
      }

      public bool SaveAll()
      {
         return dutchContext.SaveChanges() > 0;
      }
   }
}
