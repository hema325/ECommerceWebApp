namespace ECommerceWebApp.DTOs.Hubs
{
    public class SendMessageDto
    {
        public int ConversationId { get; set; }
        public long MessageId { get; set; }
        public string Value  { get;set; }
        public DateTime? TimeStamp { get; set; }
    }
}
