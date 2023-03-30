using ECommerceWebApp.Models.Review;
using System.ComponentModel.DataAnnotations;
using static DataAccess.Data.Order;

namespace ECommerceWebApp.Models.Order
{
    public class OrderDetailsViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Order Date Time")]
        public DateTime TimeStamp { get; set; }
        public Statuses Status { get; set; }
        public decimal Total { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        [Display(Name="Street Address")]
        public string StreetAddress { get; set; }
        
        [Display(Name="Postal Code")]
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public IEnumerable<OrderItemDetailsViewModel> OrderItems { get; set; }
        public class OrderItemDetailsViewModel
        {
            public int ItemId { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public string ImgUrl { get; set; }
            public string Name { get; set; }

            public AddReviewViewModel reviewModel =>
                new AddReviewViewModel
                {
                    ItemId = ItemId
                };
        }
    }
}
