namespace ECommerceWebApp.DTOs.Hubs
{
    public class ReceiveMessageDto
    {
        public int ConversationId { get; set; }
        public long MessageId { get; set; }
        public string Value { get; set; }
        public DateTime? TimeStamp { get; set; }
        public string SenderImgUrl { get; set; }
        public int SenderId { get; set; }
    }
}
