using AutoMapper;
using DataAccess.Data;
using ECommerceWebApp.DTOs.Conversatiion;

namespace ECommerceWebApp.AutoMapperProfiles
{
    public class MessageProfile:Profile
    {
        public MessageProfile()
        {
            CreateMap<Message, ConversationDto>().ForMember(model => model.SenderId, options => options.MapFrom(msg => msg.Sender.Id))
                .ForMember(dto => dto.SenderImgUrl, options => options.MapFrom(msg => msg.Sender.ImgUrl))
                .ForMember(dto => dto.SenderIsOnline, options => options.MapFrom(msg => msg.Sender.IsOnline));

        }
    }
}
