using AutoMapper;
using DataAccess.DataAccessRepository.IRepository;
using Microsoft.AspNetCore.Mvc;
using ECommerceWebApp.Areas.Admin.Models.ViewComponent;

namespace ECommerceWebApp.Areas.Admin.ViewComponents
{
    public class AdminViewComponent:ViewComponent
    {
        #region fields
        private readonly IUnitOfWork UnitOfWork;
        private readonly IMapper Mapper;
        #endregion

        #region cons
        public AdminViewComponent(IUnitOfWork unitOfWork,IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }
        #endregion

        #region actions
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await UnitOfWork.Users.FindByEmailAsync(User.Identity.Name, new[]
            {
                nameof(DataAccess.Data.User.FirstName),
                nameof(DataAccess.Data.User.LastName),
                nameof(DataAccess.Data.User.Email),
                nameof(DataAccess.Data.User.ImgUrl)
            });

            return View(model: Mapper.Map<AdminViewComponentModel>(user));
        }
        #endregion

    }
}
