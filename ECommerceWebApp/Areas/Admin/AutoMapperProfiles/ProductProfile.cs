using AutoMapper;
using DataAccess.Data;
using ECommerceWebApp.Areas.Admin.DTOs.Product;
using ECommerceWebApp.Areas.Admin.Models.Product;

namespace ECommerceWebApp.Areas.Admin.AutoMapperProfiles
{
    public class ProductProfile:Profile
    {
        public ProductProfile()
        {
            CreateMap<AddProductViewModel, Product>();
            CreateMap<Product, GetProductsDto>().ForMember(dto => dto.Category, options => options.MapFrom(product => product.Category.Name));
            CreateMap<Product, ProductDetailsViewModel>().ForMember(model => model.Category, options => options.MapFrom(product => product.Category.Name))
                .ForMember(model => model.Discount, options => options.MapFrom(product => $"{product.Discount.Value}% from {product.Discount.Start} to {product.Discount.End}"));
            CreateMap<Product, EditProductViewModel>();
            CreateMap<EditProductViewModel, Product>();
        }
    }
}
