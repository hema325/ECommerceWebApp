using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Areas.Admin.Models.Product
{
    public class AddProductViewModel
    {
        [Required]
        [StringLength(24)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Display(Name="Category")]
        public int CategoryId { get; set; }

        [Display(Name="Discount")]
        public int? DiscountId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<SelectListItem> Discounts { get; set; }
    }
}
