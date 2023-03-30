using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Areas.Identity.Models.Account
{
    public class AddAddressViewModel
    {
        [Required]
        [StringLength(24)]
        public string City { get; set; }

        [Required]
        [StringLength(24)]
        public string State { get; set; }

        [Required]
        [StringLength(24)]
        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }

        [Required]
        [StringLength(24)]
        [DataType(DataType.PostalCode)]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Required]
        [Display(Name ="Country")]
        public int CountryId { get; set; }
        public IEnumerable<SelectListItem> Countries { get; set; }
    }
}
