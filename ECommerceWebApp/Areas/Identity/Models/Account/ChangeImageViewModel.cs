using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Areas.Identity.Models.Account
{
    public class ChangeImageViewModel
    {
        [Required]
        public IFormFile Image { get; set; }
    }
}
