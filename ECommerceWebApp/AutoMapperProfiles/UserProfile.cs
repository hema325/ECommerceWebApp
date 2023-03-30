using AutoMapper;
using DataAccess.Data;
using ECommerceWebApp.Areas.Admin.Models.ViewComponent;
using ECommerceWebApp.Areas.Identity.DTOs;
using ECommerceWebApp.Areas.Identity.Models.Account;
using ECommerceWebApp.Areas.Identity.Models.SignInOut;
using ECommerceWebApp.Constrains;
using ECommerceWebApp.DTOs.Conversatiion;
using ECommerceWebApp.DTOs.User;
using ECommerceWebApp.Models.ViewComponents;

namespace ECommerceWebApp.AutoMapperProfiles
{
    public class UserProfile:Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterViewModel, User>().ForMember(user => user.LockOutEnd, options => options.MapFrom(model => DateTime.UtcNow.AddDays(-1)))
                .ForMember(user => user.LastSeen, options => options.MapFrom(model => DateTime.UtcNow))
                .ForMember(user => user.ImgUrl, options => options.MapFrom(model => DefaultImages.User));

            CreateMap<User, DetailsDto>().ForMember(dto => dto.Name, options => options.MapFrom(user => $"{user.FirstName} {user.LastName}"));
            CreateMap<User, ChangeDetailsViewModel>();
            CreateMap<ChangeDetailsViewModel, User>();

            CreateMap<User, ConversationsDto>().ForMember(model => model.ConversationId, options => options.MapFrom(user => user.Conversation.Id))
                .ForMember(model => model.UserName, options => options.MapFrom(user => $"{user.FirstName} {user.LastName}"))
                .ForMember(model => model.LastMessage, options => options.MapFrom(user => user.Conversation.LastMessage!=null? user.Conversation.LastMessage.Value:null))
                .ForMember(model => model.MessageTimeStamp, options => options.MapFrom(user => user.Conversation.LastMessage != null ? user.Conversation.LastMessage.TimeStamp:null))
                .ForMember(model => model.UnReadMessagesCount, options => options.MapFrom(user => user.Conversation.UnReadMessagesCount))
                .ForMember(model => model.UserId, options => options.MapFrom(user => user.Id));

            CreateMap<User, UserComponentViewModel>();
            CreateMap<User, ShowUsersDto>().ForMember(dto => dto.Name, options => options.MapFrom(user => $"{user.FirstName} {user.LastName}"));
            CreateMap<User, CreateGroupDto>().ForMember(dto => dto.Name, options => options.MapFrom(user => $"{user.FirstName} {user.LastName}"));
            CreateMap<User, GroupMembersDto>().ForMember(dto => dto.Name, options => options.MapFrom(user => $"{user.FirstName} {user.LastName}"));

        }
    }
}
