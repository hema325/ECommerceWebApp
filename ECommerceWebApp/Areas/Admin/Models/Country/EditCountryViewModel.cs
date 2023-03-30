using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Areas.Admin.Models.Country
{
    public class EditCountryViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(24)]
        public string Name { get; set; }
    }
}
