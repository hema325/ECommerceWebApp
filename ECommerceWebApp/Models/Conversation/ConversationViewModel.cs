namespace ECommerceWebApp.Models.Conversation
{
    public class ConversationViewModel
    {
        public int ConvId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string SenderImgUrl { get; set; }
        public DateTimeOffset ReceiverLastSeen { get; set; }
        public bool ReceiverIsOnline { get; set; }
    }

}
