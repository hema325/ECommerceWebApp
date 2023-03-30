using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Areas.Admin.Models.Variation
{
    public class EditVariationViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(24)]
        public string Name { get; set; }

        [Required]
        [Display(Name="Category")]
        public int? CategoryId { get; set; }

        public IEnumerable<DataAccess.Data.Category> Categories { get; set; }
    }
}
