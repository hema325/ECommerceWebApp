using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.Repository
{
    public class DiscountRepository:EntityRepository<Discount>,IDiscountRepository
    {
        public DiscountRepository(string connectionString) : base(connectionString) { }
    }
}
