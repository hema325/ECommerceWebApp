using AutoMapper;
using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ECommerceWebApp.Constrains;
using ECommerceWebApp.DTOs.Conversatiion;
using ECommerceWebApp.DTOs.Hubs;
using ECommerceWebApp.Hubs;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ECommerceWebApp.Models.Conversation;
using Microsoft.AspNetCore.Authorization;

namespace ECommerceWebApp.Controllers
{
    [Authorize]
    public class ConversationController : Controller
    {
        #region fields
        private readonly IUnitOfWork UnitOfWork;
        private readonly IMapper Mapper;
        private readonly IHubContext<UserHub> HubContext;
        #endregion

        #region cons
        public ConversationController(IUnitOfWork unitOfWork, IMapper mapper, IHubContext<UserHub> hubContext)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
            HubContext = hubContext;
        }
        #endregion

        #region actions

        [HttpGet]
        public IActionResult Conversations()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Conversation(int convId, int receiverId = 0)
        {
            var user = await UnitOfWork.Users.FindByIdAsync(Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                new[]
                {
                    nameof(DataAccess.Data.User.Id),
                    nameof(DataAccess.Data.User.ImgUrl)
                });

            var receiver = await UnitOfWork.Users.FindByIdAsync(receiverId, new[]
            {
                nameof(DataAccess.Data.User.IsOnline),
                nameof(DataAccess.Data.User.LastSeen)
            });

            //update messages 
            await UnitOfWork.Messages.MarkAsReadAsync(convId, user.Id);

            //notify subscribers
            await HubContext.Clients.User(receiverId.ToString()).SendAsync("MarkAllAsRead", convId);

            return View(new ConversationViewModel
            {
                ConvId = convId,
                SenderId = user.Id,
                SenderImgUrl = user.ImgUrl,
                ReceiverId = receiverId,
                ReceiverLastSeen = receiver.LastSeen,
                ReceiverIsOnline = receiver.IsOnline
            });
        }

        [HttpGet]
        public async Task<IActionResult> Group(int convId)
        {
            var user = await UnitOfWork.Users.FindByIdAsync(Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                new[]
                {
                    nameof(DataAccess.Data.User.Id),
                    nameof(DataAccess.Data.User.ImgUrl)
                });

            var userConv = await UnitOfWork.UserConversations.FindByConvIdUserId(convId, user.Id);

            //check if i'am in the group or not
            if (userConv.LeftDateTime == null)
            {
                //update messages 
                await UnitOfWork.Messages.MarkAsReadAsync(convId, user.Id);

                //get subscribers
                var subscribers = await UnitOfWork.Conversations.GetGroupOnlineMembers(convId, new[]
                {
                nameof(DataAccess.Data.User.Id)
                });

                //notify subscribers
                await HubContext.Clients.Users(subscribers.Where(sub => sub.Id != user.Id).Select(sub => sub.Id.ToString())).SendAsync("MarkAllAsRead", convId);
            }

            return View(new GroupViewModel
            {
                ConvId = convId,
                SenderId = user.Id,
                SenderImgUrl = user.ImgUrl,
                LeftDateTime = userConv.LeftDateTime
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup(CreateGroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

                //create group
                var convId = await UnitOfWork.Conversations.AddAsync<int>(new Conversation 
                {
                    Name = model.GroupName
                });

                //add users
                model.UsersIDs.Add(userId);
                var userConvs = model.UsersIDs.Select(id => new UserConversation
                {
                    ConversationId = convId,
                    UserId = id,
                    JoinDateTime = DateTime.Now
                });

                await UnitOfWork.UserConversations.AddMultipleAsync(userConvs);


                //notify subscribers
                await HubContext.Clients.Users(model.UsersIDs.Select(id => id.ToString())).SendAsync("ChangeGroupDetails", new ChangeGroupDetailsDto
                {
                    ConversationId = convId,
                    UserId = userId,
                    Name = model.GroupName,
                    ImgUrl = DefaultImages.Group
                });

                return RedirectToAction(nameof(Group), new { convId = convId });
            }

            TempData["danger"] = "Failed To Create A Group";
            return RedirectToAction(nameof(Conversations));
        }

        [HttpPost]
        public async Task<IActionResult> AddUserToGroup([Required]int convId,[Required]int chosenUserId)
        {
            if (ModelState.IsValid)
            {
                var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var addedUser = await UnitOfWork.Users.FindByIdAsync(chosenUserId, new[]
                {
                    nameof(DataAccess.Data.User.FirstName),
                    nameof(DataAccess.Data.User.LastName)
                });

                //check if user left group before
                if (!await UnitOfWork.UserConversations.IsUserExistsInConv(chosenUserId, convId))
                {
                    //add user
                    var userConv = new UserConversation
                    {
                        ConversationId = convId,
                        UserId = chosenUserId,
                        JoinDateTime = DateTime.Now
                    };

                    await UnitOfWork.UserConversations.AddAsync(userConv);
                }
                else
                {
                    await UnitOfWork.UserConversations.UpdateLeftDateTime(convId, chosenUserId, null);
                }

                //get group
                var group = await UnitOfWork.Conversations.FindByIdAsync(convId, new[]
                {
                    nameof(DataAccess.Data.Conversation.Name)
                });

                var user = await UnitOfWork.Users.FindByIdAsync(userId, new[]
                {
                    nameof(DataAccess.Data.User.ImgUrl)
                });

                var subscribers = await UnitOfWork.Conversations.GetGroupOnlineMembers(convId, new[]
                {
                    nameof(DataAccess.Data.User.Id)
                });

                await HubContext.Clients.User(chosenUserId.ToString()).SendAsync("AddedToGroup", convId);

                return RedirectToAction(nameof(Group), new { convId = convId });
            }
            
            TempData["danger"] = "Failed To Add User";
            return RedirectToAction(nameof(Conversations));
        }

        [HttpGet]
        public async Task<IActionResult> AddConversation(int userId2)
        {
            var user = await UnitOfWork.Users.FindByIdAsync(Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)), new[]
            {
                nameof(DataAccess.Data.User.Id),
                nameof(DataAccess.Data.User.FirstName),
                nameof(DataAccess.Data.User.LastName),
                nameof(DataAccess.Data.User.ImgUrl)
            });

            var convId = await UnitOfWork.Conversations.GetConvIdBetween(user.Id, userId2);

            if (convId == 0)
            {
                convId = await UnitOfWork.Conversations.AddAsync<int>(new Conversation());

                await UnitOfWork.UserConversations.AddMultipleAsync(new[]{
                    new UserConversation
                    {
                        ConversationId = convId,
                        UserId = user.Id,
                        JoinDateTime = DateTime.Now
                    },
                    new UserConversation
                    {
                        ConversationId = convId,
                        UserId = userId2,
                        JoinDateTime = DateTime.Now
                    }
                });

                //notify subscribers
                await HubContext.Clients.Users(userId2.ToString()).SendAsync("ChangeConversationDetails", new ChangeConversationDetailsDto
                {
                    ConversationId = convId,
                    UserId = user.Id,
                    Name = $"{user.FirstName} {user.LastName}",
                    ImgUrl = DefaultImages.Group,
                    LastMessage = "No Messages",
                    UnReadMessagesCount = 0
                });

            }

            return RedirectToAction(nameof(Conversation), new { convId = convId, receiverId = userId2 });
        }

        #endregion

        #region ajax

        [HttpGet]
        public async Task<IEnumerable<GroupsDto>> SearchGroups(string filter, int? skip, int? take)
        {
            var groups = await UnitOfWork.Conversations.GetGroupsByNameAsync(Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)), filter, skip, take);
            return Mapper.Map<IEnumerable<GroupsDto>>(groups);
        }

        [HttpGet]
        public async Task<IEnumerable<GroupsDto>> GetGroups(int? skip = null, int? take = null)
        {
            var groups = await UnitOfWork.Conversations.GetGroupsAsync(Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)), skip, take);
            return Mapper.Map<IEnumerable<GroupsDto>>(groups);
        }

        [HttpGet]
        public async Task<IEnumerable<ConversationsDto>> SearchConversations(string filter, int? skip = null, int? take = null)
        {
            var user = await UnitOfWork.Conversations.GetConversationsByNameAsync(Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                filter, skip, take);

            return Mapper.Map<IEnumerable<ConversationsDto>>(user);
        }

        [HttpGet]
        public async Task<IEnumerable<ConversationsDto>> GetConversations(int? skip = null, int? take = null)
        {
            var user = await UnitOfWork.Conversations.GetConversationsAsync(Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)), skip, take); ;

            return Mapper.Map<IEnumerable<ConversationsDto>>(user);
        }

        [HttpGet]
        public async Task<IEnumerable<ConversationDto>> GetConversationMsgs(int convId, int? skip = null, int? take = null)
        {
            //Get User Messages
            var messages = await UnitOfWork.Messages.GetByConvId(convId, skip: skip, take: take);

            return Mapper.Map<IEnumerable<ConversationDto>>(messages);
        }

        [HttpGet]
        public async Task<IEnumerable<ConversationDto>> GetGroupMsgs(int convId, int? skip = null, int? take = null)
        {
            //Get User Messages
            var messages = await UnitOfWork.Messages.GetByConvIdUserId(convId, Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)), skip: skip, take: take);

            return Mapper.Map<IEnumerable<ConversationDto>>(messages);
        }

        [HttpGet]
        public async Task<IEnumerable<GroupMembersDto>> GetGroupMembers(int convId)
        {
            var members = await UnitOfWork.Conversations.GetAllGroupMembers(convId, new[]
            {
                nameof(DataAccess.Data.User.Id),
                nameof(DataAccess.Data.User.FirstName),
                nameof(DataAccess.Data.User.LastName),
                nameof(DataAccess.Data.User.ImgUrl),
                nameof(DataAccess.Data.User.Email),
                nameof(DataAccess.Data.User.IsOnline)
            });

            return Mapper.Map<IEnumerable<GroupMembersDto>>(members).Select(dto=> { dto.ConversationId = convId;return dto; });
        }

        [HttpPut]
        public async Task<bool> LeaveGroup(int convId)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (await UnitOfWork.UserConversations.UpdateLeftDateTime(convId, userId, DateTime.Now))
            {
                var user = await UnitOfWork.Users.FindByIdAsync(userId, new[]
                {
                    nameof(DataAccess.Data.User.FirstName),
                    nameof(DataAccess.Data.User.LastName)
                });

                //get subscribers
                var subscribers = await UnitOfWork.Conversations.GetGroupOnlineMembers(convId, new[]
                {
                    nameof(DataAccess.Data.User.Id)
                });

                //notify subscribers
                await HubContext.Clients.Users(subscribers.Select(user=>user.Id.ToString())).SendAsync("LeftGroupMsg", convId, $"{user.FirstName} {user.LastName}");

                return true;
            }
            return false;
        }

        [HttpPut]
        public async Task MarkMessagAsRead(long msgId, int convId)
        {
            await UnitOfWork.Messages.MarkAsReadAsync(msgId);

            //get subscribers
            var users = await UnitOfWork.Conversations.GetGroupOnlineMembers(convId, new[]
            {
                nameof(DataAccess.Data.User.Id)
            });

            //notify subscribers
            await HubContext.Clients.Users(users.Select(user => user.Id.ToString())).SendAsync("MarkAsRead", msgId);
        }

        #endregion
    }
}
