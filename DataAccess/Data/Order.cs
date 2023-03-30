using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data
{
    public class Order
    {
        public enum Statuses
        {
            UnPaid,Pending, Approved,Rejected,Shipped,Delivered,Canceled
        }

        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public Statuses Status { get; set; }
        public decimal Total { get; set; }
        public string SessionId { get; set; }
        public string PaymentIntentId { get; set; }
        public int AddressId {get;set;}
        public int UserId { get; set; }

        //ignored
        public List<OrderItem> OrderItems { get; set; }
        public Address Address { get; set; }
    }
}
