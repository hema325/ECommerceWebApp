using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Models.Review
{
    public class AddReviewViewModel
    {
        public int ItemId { get; set; }

        [Required]
        [StringLength(450)]
        [Display(Name ="Review")]
        public string Comment { get; set; }
    }
}
