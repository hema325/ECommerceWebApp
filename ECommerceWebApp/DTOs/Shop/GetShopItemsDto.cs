namespace ECommerceWebApp.DTOs.Item
{
    public class GetShopItemsDto
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string ImgUrl { get; set; }
        public string Name { get; set; }
        public decimal? Discount { get; set; }
    }
}
