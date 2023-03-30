using AutoMapper;
using DataAccess.Data;
using ECommerceWebApp.DTOs.Order;
using ECommerceWebApp.Models.Order;

namespace ECommerceWebApp.AutoMapperProfiles
{
    public class AddressProfile:Profile
    {
        public AddressProfile()
        {
            CreateMap<Address, GetAddressesDto>().ForMember(dto => dto.Country, options => options.MapFrom(address => address.Country.Name));
        }
    }
}
