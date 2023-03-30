namespace ECommerceWebApp.DTOs.Shop
{
    public class ShopItemsFilterDto
    {
        public enum SortByOptions
        {
            PriceAsc,PriceDesc,ViewsAsc,ViewsDesc
        }

        public int? CategoryId { get; set; }
        public SortByOptions? SortBy { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
