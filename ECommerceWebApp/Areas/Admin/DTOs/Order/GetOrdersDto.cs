using static DataAccess.Data.Order;

namespace ECommerceWebApp.Areas.Admin.DTOs.Order
{
    public class GetOrdersDto
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Status { get; set; }
        public decimal Total { get; set; }
    }
}
