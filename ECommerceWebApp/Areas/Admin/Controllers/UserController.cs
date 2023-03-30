using AutoMapper;
using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using ECommerceWebApp.Areas.Admin.DTOs.User;
using ECommerceWebApp.Areas.Admin.Models.User;
using ECommerceWebApp.Areas.Identity.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace ECommerceWebApp.Areas.Admin.Controllers
{
    public class UserController : AdminBaseController
    {
        #region fields
        private readonly IUnitOfWork UnitOfWork;
        private readonly IMapper Mapper;
        private readonly IPasswordHasher<User> PasswordHasher;
        #endregion

        #region cons
        public UserController(IUnitOfWork unitOfWork, IMapper mapper, IPasswordHasher<User> passwordHasher)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
            PasswordHasher = passwordHasher;
        }
        #endregion

        #region actions

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var user = await UnitOfWork.Users.FindByIdIncludeAddreesesAsync(id, new[]
            {
                nameof(DataAccess.Data.User.FirstName),
                nameof(DataAccess.Data.User.LastName),
                nameof(DataAccess.Data.User.Email),
                nameof(DataAccess.Data.User.PhoneNumber),
                nameof(DataAccess.Data.User.ImgUrl)
            });

            var model = Mapper.Map<AccountIndexViewModel>(user);
            model.Addresses = user.Addresses;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var roles = await UnitOfWork.Roles.GetAllAsync();
            return View(new AddUserViewModel
            {
                Roles = roles.Select(role => new SelectListItem { Value = role.Id.ToString(), Text = role.Name })
            });
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = Mapper.Map<User>(model);
                user.Password = PasswordHasher.HashPassword(user,model.Password);
                var userId = await UnitOfWork.Users.AddAsync<int>(user);
                await UnitOfWork.UserRoles.AddAsync(new UserRole { UserId = userId, RoleId = model.RoleId });

                TempData["success"] = "User Is Added Successfully";
            }
            else
                TempData["danger"] = "Failed To Add User";


            return RedirectToAction(nameof(Add));
        }

        [HttpGet]
        public async Task<IActionResult> Block(int id)
        {
            if (await UnitOfWork.Users.UpdateAsync(new DataAccess.Data.User
            {
                Id = id,
                IsBlocked = true
            }, new[]
            {
                nameof(DataAccess.Data.User.IsBlocked)
            }))
                TempData["success"] = "User Is Blocked Successfully";
            else
                TempData["danger"] = "Failed To Block";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> UnBlock(int id)
        {
            if (await UnitOfWork.Users.UpdateAsync(new DataAccess.Data.User
            {
                Id = id,
                IsBlocked = false
            }, new[]
            {
                nameof(DataAccess.Data.User.IsBlocked)
            }))
                TempData["success"] = "User Is Blocked Successfully";
            else
                TempData["danger"] = "Failed To Block";
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region ajax
        [HttpGet]
        public  async Task<IEnumerable<GetUsersDto>> GetUsers(int? skip, int? take)
        {
            var users = await UnitOfWork.Users.GetAllAsync(new[]
            {
                nameof(DataAccess.Data.User.Id),
                nameof(DataAccess.Data.User.FirstName),
                nameof(DataAccess.Data.User.LastName),
                nameof(DataAccess.Data.User.Email),
                nameof(DataAccess.Data.User.PhoneNumber),
                nameof(DataAccess.Data.User.ImgUrl),
                nameof(DataAccess.Data.User.IsBlocked)
            }, new[]
            {
                nameof(DataAccess.Data.User.Id)
            }, skip, take);

            return Mapper.Map<IEnumerable<GetUsersDto>>(users);
        }

        [HttpGet]
        public async Task<IEnumerable<GetUsersDto>> SearchUsers(string filter,int? skip, int? take)
        {
            var users = await UnitOfWork.Users.GetByNameAsync(filter,new[]
            {
                nameof(DataAccess.Data.User.Id),
                nameof(DataAccess.Data.User.FirstName),
                nameof(DataAccess.Data.User.LastName),
                nameof(DataAccess.Data.User.Email),
                nameof(DataAccess.Data.User.PhoneNumber),
                nameof(DataAccess.Data.User.ImgUrl),
            }, new[]
            {
                nameof(DataAccess.Data.User.Id)
            }, skip, take);

            return Mapper.Map<IEnumerable<GetUsersDto>>(users);
        }

        #endregion
    }
}
