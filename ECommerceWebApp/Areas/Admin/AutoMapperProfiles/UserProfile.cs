using AutoMapper;
using DataAccess.Data;
using ECommerceWebApp.Areas.Admin.DTOs.User;
using ECommerceWebApp.Areas.Admin.Models.User;
using ECommerceWebApp.Areas.Admin.Models.ViewComponent;
using ECommerceWebApp.Constrains;

namespace ECommerceWebApp.Areas.Admin.AutoMapperProfiles
{
    public class UserProfile:Profile
    {
        public UserProfile()
        {
            CreateMap<User, AdminViewComponentModel>().ForMember(model => model.Name, options => options.MapFrom(user => $"{user.FirstName} {user.LastName}"));
            CreateMap<User, GetUsersDto>().ForMember(dto => dto.Name, options => options.MapFrom(user => $"{user.FirstName} {user.LastName}"));
            
            CreateMap<AddUserViewModel, User>().ForMember(user => user.LockOutEnd, options => options.MapFrom(model => DateTime.UtcNow.AddDays(-1)))
                .ForMember(user => user.LastSeen, options => options.MapFrom(model => DateTime.UtcNow))
                .ForMember(user => user.ImgUrl, options => options.MapFrom(model => DefaultImages.User))
                .ForMember(user => user.EmailConfirmed, options => options.MapFrom(model => true));
        }
    }
}
