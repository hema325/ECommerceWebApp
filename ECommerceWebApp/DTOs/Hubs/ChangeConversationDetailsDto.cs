namespace ECommerceWebApp.DTOs.Hubs
{
    public class ChangeConversationDetailsDto
    {
        public int ConversationId { get; set; }
        public int UserId {get;set;}
        public string Name { get; set; }
        public string ImgUrl { get; set; }
        public string LastMessage { get; set; }
        public DateTime? MessageTimeStamp { get; set; }
        public int UnReadMessagesCount { get; set; }
    }
}
