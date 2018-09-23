using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DutchTreat.Data.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace DutchTreat.Data
{
   public class DutchSeeder
   {
      private DutchContext _ctx;
      private IHostingEnvironment _hosting;
      private UserManager<StoreUser> _userManager;

      public DutchSeeder(DutchContext ctx, IHostingEnvironment hosting, UserManager<StoreUser> userManager)
      {
         this._ctx = ctx;
         this._hosting = hosting;
         this._userManager = userManager;
      }

      public void Seed()
      {
         _ctx.Database.EnsureCreated();

         var user = _userManager.FindByEmailAsync("test@test.com").Result;
         if (user == null)
         {
            user = new StoreUser
            {
               FirstName = "test",
               LastName = "test",
               Email = "test@test.com",
               UserName = "test@test.com"
            };

            var result = _userManager.CreateAsync(user, "P@ssw0rd!").Result;
            if (result != IdentityResult.Success)
            {
               throw new InvalidOperationException("Could not create new user in seeder");
            }
         }

         if (!_ctx.Products.Any())
         {
            //Need to create sample data
            var filePath = Path.Combine(_hosting.ContentRootPath, "Data//art.json");
            var json = File.ReadAllText(filePath);

            var products = JsonConvert.DeserializeObject<IEnumerable<Product>>(json);
            _ctx.Products.AddRange(products);

            var order = _ctx.Orders.Where(o => o.Id == 1).FirstOrDefault();
            if (order != null)
            {
               order.User = user;
               order.Items = new List<OrderItem>
                {
                   new OrderItem
                   {
                      Product = products.First(),
                      Quantity = 5,
                      UnitPrice = products.First().Price
                   }
                };
            }

            _ctx.SaveChanges();
         }
      }
   }
}
