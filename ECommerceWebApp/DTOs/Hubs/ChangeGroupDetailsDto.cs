namespace ECommerceWebApp.DTOs.Hubs
{
    public class ChangeGroupDetailsDto
    {
        public int ConversationId { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public string ImgUrl { get; set; }
        public string LastMessage { get; set; }
        public DateTime? LastMessageTimeStamp { get; set; }
    }
}
