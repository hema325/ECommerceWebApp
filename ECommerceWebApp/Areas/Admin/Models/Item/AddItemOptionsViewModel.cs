using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Areas.Admin.Models.Item
{
    public class AddItemOptionsViewModel
    {
        [Required]
        public int ItemId { get; set; }
        [Required]
        [Display(Name ="Options")]
        public IEnumerable<int> OptionsIDs { get; set; }

        public IEnumerable<DataAccess.Data.Variation> Variations { get; set; }
        public string ReturnUrl { get; set; }
    }
}
