using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Areas.Admin.Models.Country
{
    public class AddCountryViewModel
    {
        [Required]
        [StringLength(24)]
        public string Name { get; set; }
    }
}
