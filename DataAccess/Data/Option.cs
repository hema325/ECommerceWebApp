using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data
{
    public class Option
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public int VariationId { get; set; }

        //ignoed 
        public Variation Variation { get; set; }
    }
}
