using System.ComponentModel.DataAnnotations;

namespace DutchTreat.ViewModels
{
   public class OrderItemViewModel
   {
      public int Id { get; set; }
      [Required]
      public int Quantity { get; set; }
      [Required]
      public decimal UnitPrice { get; set; }

      [Required]
      public int ProductId { get; set; }

      //Prefixing with same entity it is referring to and automapper with pick this up
      public string ProductCategory { get; set; }
      public string ProductSize { get; set; }
      public string ProductTitle { get; set; }
      public string ProductArtist { get; set; }
      public string ProductArtId { get; set; }
   }
}