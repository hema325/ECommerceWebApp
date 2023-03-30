using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Areas.Admin.Models.Variation
{
    public class AddVariationViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name="Category")]
        public string CategoryId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}
