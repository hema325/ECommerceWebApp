using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data
{
    public class Discount
    {
        public int Id { get; set; }
        public decimal Value { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
