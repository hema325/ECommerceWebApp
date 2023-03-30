using AutoMapper;
using DataAccess.Data;
using ECommerceWebApp.Areas.Admin.DTOs.Item;
using ECommerceWebApp.Areas.Admin.Models.Item;

namespace ECommerceWebApp.Areas.Admin.AutoMapperProfiles
{
    public class ItemProfile:Profile
    {
        public ItemProfile()
        {
            CreateMap<AddItemViewModel, Item>();
            CreateMap<Item, ItemDetailsViewModel>().ForMember(model => model.Product, options => options.MapFrom(item => item.Product.Name));
            CreateMap<Item, EditItemViewModel>();
            CreateMap<EditItemViewModel, Item>();
            CreateMap<Item, GetItemsDto>().ForMember(dto=>dto.Name,options=>options.MapFrom(item=>item.Product.Name))
                .ForMember(dto => dto.Discount, options => options.MapFrom(item => item.Product.Discount.Value));
        }
    }
}
