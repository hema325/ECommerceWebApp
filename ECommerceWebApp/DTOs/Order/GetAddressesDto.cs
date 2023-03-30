namespace ECommerceWebApp.DTOs.Order
{
    public class GetAddressesDto
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string StreetAddress { get; set; }
        public string PostalCode { get; set; }
    }
}
