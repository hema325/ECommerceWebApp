using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Areas.Admin.Models.Item
{
    public class AddItemViewModel
    {
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Range(0,100)]
        public decimal Discount { get; set; }
        [Required]
        public IFormFile Image { get; set; }
        [Required]
        [Display(Name ="Product")]
        public int? ProductId { get; set; }

        public IEnumerable<SelectListItem> Products { get; set; }
    }
}
