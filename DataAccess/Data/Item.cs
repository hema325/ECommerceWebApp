using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data
{
    public class Item
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int Views { get; set; }
        public string ImgUrl { get; set; }
        public int? ProductId { get; set; }

        //ignored
        public Product Product { get; set; }
        public List<Option> Options { get; set; }
    }
}
