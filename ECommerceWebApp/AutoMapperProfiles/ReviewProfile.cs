using AutoMapper;
using DataAccess.Data;
using ECommerceWebApp.DTOs.Review;
using ECommerceWebApp.Models.Review;

namespace ECommerceWebApp.AutoMapperProfiles
{
    public class ReviewProfile:Profile
    {
        public ReviewProfile()
        {
            CreateMap<AddReviewViewModel, Review>();
            CreateMap<Review, GetItemReviewsDto>()
                .ForMember(dto => dto.Name, options => options.MapFrom(review => $"{review.User.FirstName} {review.User.LastName}"))
                .ForMember(dto => dto.ImgUrl, options => options.MapFrom(review => review.User.ImgUrl));
        }
    }
}
