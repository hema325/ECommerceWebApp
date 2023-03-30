using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Areas.Identity.Models.Account
{
    public class ChangeDetailsViewModel
    {
        [Required]
        [StringLength(24)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(24)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }
}
