using AutoMapper;
using DataAccess.Data;
using ECommerceWebApp.Models.Order;
using ECommerceWebApp.Models.ShoppingCart;

namespace ECommerceWebApp.AutoMapperProfiles
{
    public class CartItemProfile:Profile
    {
        public CartItemProfile()
        {
            CreateMap<CartItem, ShoppingCartIndexViewModel>().ForMember(model => model.Name,options=>options.MapFrom(cartItem => cartItem.Item.Product.Name))
                .ForMember(model => model.Price, options => options.MapFrom(cartItem => cartItem.Item.Price))
                .ForMember(model => model.ImgUrl, options => options.MapFrom(cartItem => cartItem.Item.ImgUrl))
                .ForMember(model => model.ItemId, options => options.MapFrom(cartItem => cartItem.Item.Id))
                .ForMember(model => model.Name, options => options.MapFrom(cartItem => cartItem.Item.Product.Name))             
                .ForMember(model => model.Discount, options => options.MapFrom(cartItem => cartItem.Item.Product.Discount.Value));

        }
    }
}
