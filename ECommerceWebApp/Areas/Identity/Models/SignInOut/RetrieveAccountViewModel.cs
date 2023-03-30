using DataAccess.Data;
using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Areas.Identity.Models.SignInOut
{
    public class RetrieveAccountViewModel
    {
        [Required]
        public int Id { get; set; }


        [Required]
        [StringLength(24, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required]
        [Compare(nameof(NewPassword))]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        public string ConfirmNewPassword { get; set; }

        [Required]
        public string TokenValue { get; set; }

        [Required]
        public Token.Purposes Purpose { get; set; }
    }
}
