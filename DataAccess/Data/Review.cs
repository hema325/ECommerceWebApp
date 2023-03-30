using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data
{
    public class Review
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int ItemId { get; set; }
        public int UserId { get; set; }

        //ignored
        public User User { get; set; }
    }
}
