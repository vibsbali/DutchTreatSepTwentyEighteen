using System.Collections.Generic;
using DutchTreat.Data.Entities;

namespace DutchTreat.Data
{
   public interface IDutchRepository
   {
      IEnumerable<Product> GetAllProducts();
      IEnumerable<Product> GetProductsByCategory(string category);

      bool SaveAll();
      Order GetOrderById(string username, int id);
      void AddEntity(object model);
      IEnumerable<Order> GetAllOrdersByUser(string username, bool includeItems);
   }
}