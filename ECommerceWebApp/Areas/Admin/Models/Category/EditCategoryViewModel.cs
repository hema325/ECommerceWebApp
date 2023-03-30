using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Areas.Admin.Models.Category
{
    public class EditCategoryViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(24)]
        [Display(Name = "Category Name")]
        public string Name { get; set; }

        [Display(Name = "This Is Sub Category For")]
        [ValidateNever]
        public int? ParentId { get; set; }

        public IEnumerable<DataAccess.Data.Category> Categories { get; set; }
    }
}
