namespace ECommerceWebApp.Models.Conversation
{
    public class GroupViewModel
    {
        public int ConvId { get; set; }
        public int SenderId { get; set; }
        public string SenderImgUrl { get; set; }
        public DateTime? LeftDateTime { get; set; }
    }
}
