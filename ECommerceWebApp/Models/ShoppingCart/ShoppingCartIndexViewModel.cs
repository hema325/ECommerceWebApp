namespace ECommerceWebApp.Models.ShoppingCart
{
    public class ShoppingCartIndexViewModel
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string ImgUrl { get; set; }
        public string Name { get; set; }
        public decimal Discount { get; set; }
        public int Quantity { get; set; }
        public int ItemId { get; set; }
    }
}
