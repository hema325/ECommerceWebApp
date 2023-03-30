namespace ECommerceWebApp.DTOs.Conversatiion
{
    public class GroupsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImgUrl { get; set; }
        public string LastMessage { get; set; }
        public DateTimeOffset? MessageTimeStamp { get; set; }
        public int UnReadMessagesCount { get; set; }
    }
}
