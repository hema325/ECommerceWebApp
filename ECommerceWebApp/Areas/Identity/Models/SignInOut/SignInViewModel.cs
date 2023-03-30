using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Areas.Identity.Models.SignInOut
{
    public class SignInViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(24)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remeber Me")]
        public bool RememberMe { get; set; }
    }
}
