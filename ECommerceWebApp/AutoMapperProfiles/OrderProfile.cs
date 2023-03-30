using AutoMapper;
using DataAccess.Data;
using ECommerceWebApp.Models.Order;

namespace ECommerceWebApp.AutoMapperProfiles
{
    public class OrderProfile:Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderIndexViewModel>();
            CreateMap<Order, OrderDetailsViewModel>()
                .ForMember(model=>model.City,options=>options.MapFrom(order=>order.Address.City))
                .ForMember(model => model.State, options => options.MapFrom(order => order.Address.State))
                .ForMember(model => model.StreetAddress, options => options.MapFrom(order => order.Address.StreetAddress))
                .ForMember(model => model.PostalCode, options => options.MapFrom(order => order.Address.PostalCode))
                .ForMember(model => model.Country, options => options.MapFrom(order => order.Address.Country.Name));

            CreateMap<OrderItem, OrderDetailsViewModel.OrderItemDetailsViewModel>()
                .ForMember(model => model.ImgUrl, options => options.MapFrom(order => order.Item.ImgUrl))
                .ForMember(model => model.Name, options => options.MapFrom(order => order.Item.Product.Name))
                .ForMember(model => model.ItemId, options => options.MapFrom(order => order.Item.Id));
        }
    }
}
