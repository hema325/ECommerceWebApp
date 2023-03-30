using AutoMapper;
using DataAccess.Data;
using ECommerceWebApp.Areas.Admin.Models.Option;

namespace ECommerceWebApp.Areas.Admin.AutoMapperProfiles
{
    public class OptionProfile:Profile
    {
        public OptionProfile()
        {
            CreateMap<AddOptionViewModel, Option>();
            CreateMap<EditOptionViewModel, Option>();
            CreateMap<Option, EditOptionViewModel>();
        }
    }
}
