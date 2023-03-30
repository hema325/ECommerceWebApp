using AutoMapper;
using DataAccess.DataAccessRepository.IRepository;
using ECommerceWebApp.Constrains;
using ECommerceWebApp.Models.ViewComponents;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWebApp.ViewComponents
{
    public class UserViewComponent:ViewComponent
    {
        #region fields

        private readonly IUnitOfWork UnitOfWork;
        private readonly IMapper Mapper;

        #endregion

        #region cons

        public UserViewComponent(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }

        #endregion

        public async Task<IViewComponentResult> InvokeAsync(int userId)
        {

            var component = new UserComponentViewModel
            {
                ImgUrl = HttpContext.Session.GetString(SessionKeys.UserImgUrl),
                ConvsHasUnReadMsgsCount = await UnitOfWork.Conversations.GetConvsHasUnReadMsgsCountAsync(userId)
            };
           
            if (component.ImgUrl == null) 
            {
                var user = await UnitOfWork.Users.FindByIdAsync(userId, new[]
                {
                    nameof(DataAccess.Data.User.ImgUrl)
                });
                component.ImgUrl = user.ImgUrl;
                HttpContext.Session.SetString(SessionKeys.UserImgUrl,user.ImgUrl);
            }

            return View(model: Mapper.Map<UserComponentViewModel>(component));
        }

    }
}
