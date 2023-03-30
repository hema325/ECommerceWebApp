using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Areas.Admin.Models.Product
{
    public class EditProductViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(24)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "Discount")]
        public int? DiscountId { get; set; }

        public IEnumerable<DataAccess.Data.Category> Categories { get; set; }

        public IEnumerable<DataAccess.Data.Discount> Discounts { get; set; }
    }
}
