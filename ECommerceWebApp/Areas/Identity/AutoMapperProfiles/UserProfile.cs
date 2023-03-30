using AutoMapper;
using DataAccess.Data;
using ECommerceWebApp.Areas.Identity.Models.Account;

namespace ECommerceWebApp.Areas.Identity.AutoMapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, AccountIndexViewModel>().ForMember(model => model.Name, options => options.MapFrom(user => $"{user.FirstName} {user.LastName}"));
        }
    }
}
