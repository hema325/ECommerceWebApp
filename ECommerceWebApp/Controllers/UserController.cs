using AutoMapper;
using DataAccess.DataAccessRepository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ECommerceWebApp.DTOs.Conversatiion;
using ECommerceWebApp.DTOs.User;
using ECommerceWebApp.Hubs;
using System.Security.Claims;

namespace ECommerceWebApp.Controllers
{
    public class UserController:Controller
    {
        #region fields

        private readonly IUnitOfWork UnitOfWork;
        private readonly IMapper Mapper;

        #endregion

        #region cons

        public UserController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }

        #endregion

        #region actions

        [HttpGet]
        public IActionResult ShowUsers()
        {
            return View();
        }

        #endregion

        #region ajax

        [HttpGet]
        public async Task<IEnumerable<ShowUsersDto>> GetUsers(int? skip = null, int? take = null)
        {
            var users = await UnitOfWork.Users.GetAllAsync(new[]
            {
                nameof(DataAccess.Data.User.Id),
                nameof(DataAccess.Data.User.FirstName),
                nameof(DataAccess.Data.User.LastName),
                nameof(DataAccess.Data.User.ImgUrl)
            }, new[]
            {
                nameof(DataAccess.Data.User.Id)
            }, skip: skip, take: take);

            return Mapper.Map<IEnumerable<ShowUsersDto>>(users);
        }

        public async Task<IEnumerable<ShowUsersDto>> SearchUsers(string filter, int? skip = null, int? take = null)
        {
            var users = await UnitOfWork.Users.GetByNameAsync(filter, new[]
            {
                nameof(DataAccess.Data.User.Id),
                nameof(DataAccess.Data.User.FirstName),
                nameof(DataAccess.Data.User.LastName),
                nameof(DataAccess.Data.User.ImgUrl),
            }, new[]
            {
                nameof(DataAccess.Data.User.Id)
            }, skip: skip, take: take);

            return Mapper.Map<IEnumerable<ShowUsersDto>>(users);
        }

        [HttpGet]
        public async Task<IEnumerable<CreateGroupDto>> GetRelatedUsers()
        {
            var users = await UnitOfWork.Users.GetRelatedUsersAsync(Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)), new[]
            {
                nameof(DataAccess.Data.User.Id),
                nameof(DataAccess.Data.User.FirstName),
                nameof(DataAccess.Data.User.LastName)
            });

            return Mapper.Map<IEnumerable<CreateGroupDto>>(users);
        }

        [HttpGet]
        public async Task<IEnumerable<CreateGroupDto>> GetRelatedUsersNotIn(int convId)
        {
            var users = await UnitOfWork.Users.GetRelatedUsersNotInConvIdAsync(Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)),convId, new[]
            {
                nameof(DataAccess.Data.User.Id),
                nameof(DataAccess.Data.User.FirstName),
                nameof(DataAccess.Data.User.LastName)
            });

            return Mapper.Map<IEnumerable<CreateGroupDto>>(users);
        }

        #endregion

    }
}
