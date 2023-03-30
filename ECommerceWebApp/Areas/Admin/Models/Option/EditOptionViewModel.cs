using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Areas.Admin.Models.Option
{
    public class EditOptionViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(24)]
        public string Value { get; set; }

        public int VariationId { get; set; }
    }
}
