using DataAccess.Data;
using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Areas.Identity.Models.SignInOut
{
    public class SendVerificationViewModel
    {
        public enum Methods
        {
            Email, PhoneNumber
        }

        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(24)]
        public string Email { get; set; }

        [Required]
        public Methods Method { get; set; }

        [Required]
        public Token.Purposes Purpose { get; set; }
    }
}
