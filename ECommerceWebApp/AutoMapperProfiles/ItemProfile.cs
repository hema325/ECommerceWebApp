using AutoMapper;
using DataAccess.Data;
using ECommerceWebApp.DTOs.Item;
using ECommerceWebApp.Models.Order;
using ECommerceWebApp.Models.Shop;
using ECommerceWebApp.Models.ShoppingCart;

namespace ECommerceWebApp.AutoMapperProfiles
{
    public class ItemProfile:Profile
    {
        public ItemProfile()
        {
            CreateMap<Item, ShopItemDetailsViewModel>().ForMember(model => model.Name, options => options.MapFrom(item => item.Product.Name))
                .ForMember(model => model.Description, options => options.MapFrom(item => item.Product.Description))
                .ForMember(model => model.Discount, options => options.MapFrom(item => item.Product.Discount.Value));

            CreateMap<Item, GetShopItemsDto>().ForMember(dto => dto.Name, options => options.MapFrom(item => item.Product.Name))
                .ForMember(dto => dto.Discount, options => options.MapFrom(item => item.Product.Discount.Value));

            CreateMap<Item, ShoppingCartIndexViewModel>().ForMember(model => model.Name, options => options.MapFrom(item => item.Product.Name))
                .ForMember(model => model.Discount, options => options.MapFrom(item => item.Product.Discount.Value));

        }
    }
}
