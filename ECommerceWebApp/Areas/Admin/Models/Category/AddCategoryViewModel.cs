using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Areas.Admin.Models.Category
{
    public class AddCategoryViewModel
    {
        [Required]
        [StringLength(24)]
        [Display(Name="Category Name")]
        public string Name { get; set; }

        [Display(Name="This Is Sub Category For")]
        public int? ParentId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}
