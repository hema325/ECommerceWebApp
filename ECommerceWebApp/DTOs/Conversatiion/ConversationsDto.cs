namespace ECommerceWebApp.DTOs.Conversatiion
{
    public class ConversationsDto
    {
        public int ConversationId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string ImgUrl { get; set; }
        public bool IsOnline { get; set; }
        public string LastMessage { get; set; }
        public DateTimeOffset? MessageTimeStamp { get; set; }
        public int UnReadMessagesCount { get; set; }
    }
}
