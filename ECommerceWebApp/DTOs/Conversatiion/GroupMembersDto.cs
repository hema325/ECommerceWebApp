namespace ECommerceWebApp.DTOs.Conversatiion
{
    public class GroupMembersDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImgUrl { get; set; }
        public string Email { get; set; }
        public int ConversationId { get; set; }
        public bool IsOnline { get; set; }
    }
}
