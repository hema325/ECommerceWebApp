namespace ECommerceWebApp.Areas.Admin.Models.Item
{
    public class ItemDetailsViewModel
    {
        public int Id { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
        public decimal Price { get; set; }
        public string ImgUrl { get; set; }

        public List<DataAccess.Data.Variation> Variations { get; set; }
       
    }
}