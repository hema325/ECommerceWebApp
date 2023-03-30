using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data
{
    public class Address
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string StreetAddress { get; set; }
        public string PostalCode { get; set; }
        public int CountryId { get; set; }
        public int UserId { get; set; }

        //ignored
        public Country Country { get; set; }
    }
}
