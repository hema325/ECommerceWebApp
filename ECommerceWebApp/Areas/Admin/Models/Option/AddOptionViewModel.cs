using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Areas.Admin.Models.Option
{
    public class AddOptionViewModel
    {
        [Required]
        [StringLength(24)]
        public string Value { get; set; }
        [Required]
        public int VariationId { get; set; }
    }
}
