using DataAccess.Data;

namespace ECommerceWebApp.Areas.Admin.Models.Variation
{
    public class VariationDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public IEnumerable<DataAccess.Data.Option> Options { get; set; }
    }
}
