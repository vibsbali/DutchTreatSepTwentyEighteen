using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DutchTreat.Data.Entities;
using Microsoft.EntityFrameworkCore;
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

      public IEnumerable<Order> GetAllOrders(bool includeItems)
      {
         if (includeItems)
         {
            return dutchContext.Orders
               .Include(o => o.Items)
               .ThenInclude(o => o.Product).ToList();
         }

         return dutchContext.Orders.ToList();

      }

      public Order GetOrderById(string username, int id)
      {
         return dutchContext.Orders
            .Include(o => o.Items)
            .ThenInclude(o => o.Product)
            .FirstOrDefault(o => o.Id == id && o.User.UserName == username);
      }

      public void AddEntity(object model)
      {
         dutchContext.Add(model);
      }

      public IEnumerable<Order> GetAllOrdersByUser(string username, bool includeItems)
      {
         if (includeItems)
         {
            return dutchContext.Orders
               .Where(o => o.User.UserName == username)
               .Include(o => o.Items)
               .ThenInclude(o => o.Product).ToList();
         }

         return dutchContext.Orders.Where(o => o.User.UserName == username).ToList();
      }
   }
}
