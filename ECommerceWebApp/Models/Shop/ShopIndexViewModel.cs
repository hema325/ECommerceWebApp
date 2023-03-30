using DataAccess.Data;

namespace ECommerceWebApp.Models.Shop
{
    public class ShopIndexViewModel
    {
        public int? CategoryId { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
