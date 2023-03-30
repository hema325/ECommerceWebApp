namespace ECommerceWebApp.DTOs.Conversatiion
{
    public class ConversationDto
    {
        //message
        public long Id { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public string Value { get; set; }
        public bool IsRead { get; set; }

        //sender 
        public int SenderId { get; set; }
        public string SenderImgUrl { get; set; }
        public bool SenderIsOnline { get; set; }
    }
}
