using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Areas.Admin.Models.Product
{
    public class ProductDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Discount { get; set; }
    }
}
