using DataAccess.Data;

namespace ECommerceWebApp.Models.Shop
{
    public class ShopItemDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set;}
        public decimal Price { get; set; }
        public string ImgUrl { get; set; }
        public string Description { get; set; }
        public decimal? Discount { get; set; }
        public int Views { get; set; }
        public int CategoryId { get; set; }
        public IEnumerable<Variation> Variations { get; set; }
    }
}
