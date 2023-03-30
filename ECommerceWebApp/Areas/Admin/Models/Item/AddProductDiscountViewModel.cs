using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Areas.Admin.Models.Item
{
    public class AddProductDiscountViewModel
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Display(Name ="Discounts")]
        public IEnumerable<int> DiscountId { get; set; }

        public IEnumerable<SelectListItem> Discounts { get; set; }
    }
}
