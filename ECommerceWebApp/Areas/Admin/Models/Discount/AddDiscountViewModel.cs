using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Areas.Admin.Models.Discount
{
    public class AddDiscountViewModel
    {
        [Required]
        [Range(0,100)]
        [Display(Name ="Discount")]
        public decimal Value { get; set; }

        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }
    }
}
