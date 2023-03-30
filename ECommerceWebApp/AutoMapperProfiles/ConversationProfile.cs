using AutoMapper;
using DataAccess.Data;
using ECommerceWebApp.Constrains;
using ECommerceWebApp.DTOs.Conversatiion;

namespace ECommerceWebApp.AutoMapperProfiles
{
    public class ConversationProfile:Profile
    {
        public ConversationProfile()
        {
            CreateMap<Conversation, GroupsDto>()
                .ForMember(dto => dto.ImgUrl, options => options.MapFrom(conv => DefaultImages.Group))
                .ForMember(dto => dto.LastMessage, options => options.MapFrom(conv => conv.LastMessage.Value))
                .ForMember(dto => dto.MessageTimeStamp, options => options.MapFrom(conv => conv.LastMessage.TimeStamp));

        }
    }
}
