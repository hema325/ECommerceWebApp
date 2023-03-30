namespace ECommerceWebApp.Areas.Admin.Models.Discount
{
    public class DiscountDetailsViewModel
    {
        public int Id { get; set; }
        public decimal Value { get; set; }
        
        public DateTime Start { get; set; }

        public DateTime End { get; set; }
    }
}
