using AutoMapper;
using DataAccess.Data;
using ECommerceWebApp.Areas.Admin.DTOs.Country;
using ECommerceWebApp.Areas.Admin.Models.Country;

namespace ECommerceWebApp.Areas.Admin.AutoMapperProfiles
{
    public class CountryProfile:Profile
    {
        public CountryProfile()
        {
            CreateMap<AddCountryViewModel, Country>();
            CreateMap<Country, EditCountryViewModel>();
            CreateMap<EditCountryViewModel, Country>();
            CreateMap<Country, GetCountriesDto>();
        }
    }
}
