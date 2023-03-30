using static DataAccess.Data.Order;

namespace ECommerceWebApp.Models.Order
{
    public class OrderIndexViewModel
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public Statuses Status { get; set; }
        public decimal Total { get; set; }
    }
}
