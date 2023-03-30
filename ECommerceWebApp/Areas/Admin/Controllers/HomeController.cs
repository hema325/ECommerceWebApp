using DataAccess.DataAccessRepository.IRepository;
using ECommerceWebApp.Areas.Admin.Models.Home;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWebApp.Areas.Admin.Controllers
{
    public class HomeController :AdminBaseController
    {
        #region fields
        private readonly IUnitOfWork UnitOfWork;
        #endregion

        #region cons
        public HomeController(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
        #endregion

        #region actions
        public async Task<IActionResult> Index()
        {
            return View(new HomeIndexViewModel
            {
                Users = await UnitOfWork.Users.CountAsync(),
                Products = await UnitOfWork.Products.CountAsync(),
                Items = await UnitOfWork.Items.CountAsync(),
                Orders = await UnitOfWork.Orders.CountAsync()
            });
        }
        #endregion
    }
}
