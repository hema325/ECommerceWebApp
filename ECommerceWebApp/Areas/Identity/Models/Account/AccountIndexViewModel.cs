using DataAccess.Data;
using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Areas.Identity.Models.Account
{
    public class AccountIndexViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        public string ImgUrl { get; set; }

        public IEnumerable<Address> Addresses { get; set; }

    }
}
