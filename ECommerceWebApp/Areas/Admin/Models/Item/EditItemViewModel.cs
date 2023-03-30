using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Areas.Admin.Models.Item
{
    public class EditItemViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }
        [Range(0, 100)]
        public decimal Discount { get; set; }

        [Required]
        public string ImgUrl { get; set; }

        public IFormFile Image { get; set; }

        [Required]
        [Display(Name = "Product")]
        public int ProductId { get; set; }

        [Required]
        public int ItemId { get; set; }

        public IEnumerable<DataAccess.Data.Product> Products { get; set; }
    }
}
