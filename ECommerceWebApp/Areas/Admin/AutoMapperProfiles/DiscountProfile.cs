using AutoMapper;
using DataAccess.Data;
using ECommerceWebApp.Areas.Admin.DTOs.Discount;
using ECommerceWebApp.Areas.Admin.Models.Discount;

namespace ECommerceWebApp.Areas.Admin.AutoMapperProfiles
{
    public class DiscountProfile:Profile
    {
        public DiscountProfile()
        {
            CreateMap<AddDiscountViewModel, Discount>();
            CreateMap<EditDiscountViewModel, Discount>();
            CreateMap<Discount, EditDiscountViewModel>();
            CreateMap<Discount,DiscountDetailsViewModel>();
            CreateMap<Discount, GetDiscountsDto>();
        }
    }
}
