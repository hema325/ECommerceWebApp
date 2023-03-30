using AutoMapper;
using DataAccess.Data;
using ECommerceWebApp.Areas.Admin.DTOs.Variation;
using ECommerceWebApp.Areas.Admin.Models.Variation;

namespace ECommerceWebApp.Areas.Admin.AutoMapperProfiles
{
    public class VariationProfile:Profile
    {
        public VariationProfile()
        {
            CreateMap<AddVariationViewModel,Variation>();
            CreateMap<Variation, GetVariationsDto>().ForMember(dto => dto.Category, options => options.MapFrom(variation => variation.Category.Name));
            CreateMap<Variation, EditVariationViewModel>();
            CreateMap<EditVariationViewModel, Variation>();
            CreateMap<Variation, VariationDetailsViewModel>().ForMember(model => model.Category, options => options.MapFrom(variation => variation.Category.Name));
        }
    }
}
