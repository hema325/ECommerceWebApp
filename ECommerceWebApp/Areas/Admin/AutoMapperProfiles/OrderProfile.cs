using AutoMapper;
using DataAccess.Data;
using ECommerceWebApp.Areas.Admin.DTOs.Order;

namespace ECommerceWebApp.Areas.Admin.AutoMapperProfiles
{
    public class OrderProfile:Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, GetOrdersDto>();
        }
    }
}
