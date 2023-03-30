using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Models.Conversation
{
    public class CreateGroupViewModel
    {
        [Required]
        [Display(Name = "Group Name")]
        [StringLength(24)]
        public string GroupName { get; set; }

        [Required]
        public List<int> UsersIDs { get; set; }
    }
}
