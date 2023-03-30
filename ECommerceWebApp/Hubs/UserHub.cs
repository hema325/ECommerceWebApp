using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using Microsoft.AspNetCore.SignalR;
using ECommerceWebApp.Constrains;
using ECommerceWebApp.DTOs.Hubs;

namespace ECommerceWebApp.Hubs
{
    public class UserHub:Hub
    {
        private readonly IUnitOfWork UnitOfWork;

        public UserHub(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public override async Task OnConnectedAsync()
        {
            await UnitOfWork.Users.UpdateAsync(new User
            {
                Id = Convert.ToInt32(Context.UserIdentifier),
                IsOnline = true
            }, new[] 
            {
                nameof(User.IsOnline)
            });

            await Clients.All.SendAsync("setUserOnline", Context.UserIdentifier);
        }

        public override async Task OnDisconnectedAsync(Exception exp)
        {
            await UnitOfWork.Users.UpdateAsync(new User
            {
                Id = Convert.ToInt32(Context.UserIdentifier),
                IsOnline = false,
                LastSeen = DateTime.Now
            }, new[]
            {
                nameof(User.IsOnline),
                nameof(User.LastSeen)
            });

            await Clients.All.SendAsync("setUserOffline", Context.UserIdentifier);
        }


        public async Task SendPrivate(string convId,string receiverId, string msgValue)
        {
            var senderId = Convert.ToInt32(Context.UserIdentifier);

            //create message
            var message = new Message
            {
                TimeStamp = DateTime.Now,
                Value = msgValue,
                SenderId = senderId,
                ConversationId = Convert.ToInt32(convId),
                IsRead = false
            };

            // save to db
            message.Id = await UnitOfWork.Messages.AddAsync<long>(message);

            //update last message
            await UnitOfWork.Conversations.UpdateAsync(
                new Conversation
                {
                    Id = message.ConversationId,
                    LastMessageId = message.Id
                },
                new[]
                {
                    nameof(Conversation.LastMessageId)
                });

            //get msg sender details
            var sender = await UnitOfWork.Users.FindByIdAsync(senderId, new[]
            {
                nameof(DataAccess.Data.User.FirstName),
                nameof(DataAccess.Data.User.LastName),
                nameof(DataAccess.Data.User.ImgUrl)
            });

            //notify subscribers

            await Clients.User(receiverId).SendAsync("RecieveMessage", new ReceiveMessageDto
            {
                ConversationId = Convert.ToInt32(convId),
                MessageId = message.Id,
                Value = message.Value,
                TimeStamp = message.TimeStamp,
                SenderImgUrl = sender.ImgUrl,
                SenderId = senderId
            });

            await Clients.Caller.SendAsync("SendMessage", new SendMessageDto
            {
                ConversationId = Convert.ToInt32(convId),
                MessageId = message.Id,
                Value = message.Value,
                TimeStamp = message.TimeStamp
            });

            await Clients.User(receiverId).SendAsync("ChangeConversationDetails", new ChangeConversationDetailsDto
            {
                ConversationId = Convert.ToInt32(convId),
                UserId = senderId,
                Name = $"{sender.FirstName} {sender.LastName}",
                ImgUrl = sender.ImgUrl,
                LastMessage = message.Value,
                MessageTimeStamp = message.TimeStamp,
                UnReadMessagesCount = await UnitOfWork.Messages.GetConvHasUnReadMsgsCount(message.ConversationId, Convert.ToInt32(receiverId))
            });

        }

        
        public async Task SendGroup(string convId, string msgValue)
        {
            var senderId = Convert.ToInt32(Context.UserIdentifier);

            var sender = await UnitOfWork.Users.FindByIdAsync(senderId, new[]
            {
                nameof(DataAccess.Data.User.ImgUrl)
            });

            //get online members
            var receivers = await UnitOfWork.Conversations.GetGroupOnlineMembers(Convert.ToInt32(convId), new[]
            {
                nameof(DataAccess.Data.User.Id)
            });


            //create message
            var message = new Message
            {
                TimeStamp = DateTime.Now,
                Value = msgValue,
                SenderId = senderId,
                ConversationId = Convert.ToInt32(convId),
                IsRead = false
            };

            // save to db
            message.Id = await UnitOfWork.Messages.AddAsync<long>(message);

            //update last message
            await UnitOfWork.Conversations.UpdateAsync(
                new Conversation
                {
                    Id = message.ConversationId,
                    LastMessageId = message.Id
                },
                new[]
                {
                    nameof(Conversation.LastMessageId)
                });

            //get conversation
            var conv = await UnitOfWork.Conversations.FindByIdAsync(Convert.ToInt32(convId), new[] { nameof(Conversation.Name) });

            //notify subscribers

            var subscribers = receivers.Where(user => user.Id != senderId).Select(user => user.Id.ToString());

            await Clients.Users(subscribers).SendAsync("RecieveMessage", new ReceiveMessageDto
            {
                ConversationId = Convert.ToInt32(convId),
                MessageId = message.Id,
                Value = message.Value,
                TimeStamp = message.TimeStamp,
                SenderImgUrl = sender.ImgUrl,
                SenderId = senderId
            });

            await Clients.Users(subscribers).SendAsync("ChangeGroupDetails", new ChangeGroupDetailsDto
            {
                ConversationId = Convert.ToInt32(convId),
                UserId = senderId,
                Name = conv.Name,
                ImgUrl = DefaultImages.Group,
                LastMessage = message.Value,
                LastMessageTimeStamp = message.TimeStamp
            });

            await Clients.Caller.SendAsync("SendMessage", new SendMessageDto
            {
                ConversationId = Convert.ToInt32(convId),
                MessageId = message.Id,
                Value = message.Value,
                TimeStamp = message.TimeStamp
            });
        }

    }
}
