using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Areas.Admin.Models.Category
{
    public class CategoryDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Display(Name="This Category Is Sub For ")]
        public string ParentName { get; set; }
    }
}
