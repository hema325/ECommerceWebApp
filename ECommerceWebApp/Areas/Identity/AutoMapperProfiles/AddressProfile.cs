using AutoMapper;
using DataAccess.Data;
using ECommerceWebApp.Areas.Identity.Models.Account;

namespace ECommerceWebApp.Areas.Identity.AutoMapperProfiles
{
    public class AddressProfile:Profile
    {
        public AddressProfile()
        {
            CreateMap<AddAddressViewModel, Address>();
        }
        
    }
}
