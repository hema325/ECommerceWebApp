namespace ECommerceWebApp.Areas.Admin.DTOs.Discount
{
    public class GetDiscountsDto
    {
        public int Id { get; set; }
        public decimal Value { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
