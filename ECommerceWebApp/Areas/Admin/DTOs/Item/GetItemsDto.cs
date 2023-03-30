namespace ECommerceWebApp.Areas.Admin.DTOs.Item
{
    public class GetItemsDto
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string ImgUrl { get; set; }
        public string Name { get; set; }
        public decimal? Discount { get; set; }
        public int Quantity { get; set; }
    }
}
