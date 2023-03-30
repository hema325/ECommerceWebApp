using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.Repository
{
    public class AddressRepository:EntityRepository<Address>,IAddressRepository
    {
        public AddressRepository(string connectionString) : base(connectionString) 
        {
            IgnoredProps.Add(nameof(Address.Country));
        }

        public async Task<IEnumerable<Address>> GetByUserId(int userId, IEnumerable<string> attrs = null)
        {
            if (attrs == null)
                attrs = EntityProps;

            var sql = @$"select {String.Join(",", attrs.Select(attr=>$"ADs.{attr}"))},Cs.* from addresses as ADs
                         left join Countries as Cs on Cs.Id = ADs.countryId
                         where ADs.userId = @userId";

            return await QueryAsync<Address, Country, Address>(sql, (address, country) =>
            {
                address.Country = country;
                return address;
            } ,new {userId = userId});
        }
    }
}
