namespace ECommerceWebApp.Areas.Admin.DTOs.User
{
    public class GetUsersDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ImgUrl { get; set; }
        public bool IsBlocked { get; set; }

    }
}
