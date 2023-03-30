using AutoMapper;
using DataAccess.Data;
using ECommerceWebApp.Areas.Admin.DTOs.Category;
using ECommerceWebApp.Areas.Admin.Models.Category;

namespace ECommerceWebApp.Areas.Admin.AutoMapperProfiles
{
    public class CategoryProfile:Profile
    {
        public CategoryProfile()
        {
            CreateMap<AddCategoryViewModel, Category>();
            CreateMap<EditCategoryViewModel, Category>();
            CreateMap<Category, EditCategoryViewModel>();
            CreateMap<Category, GetCategoriesDto>();

            CreateMap<Category, CategoryDetailsViewModel>().ForMember(model => model.ParentName, options => options.MapFrom(category => category.Parent.Name));
        }
    }
}
