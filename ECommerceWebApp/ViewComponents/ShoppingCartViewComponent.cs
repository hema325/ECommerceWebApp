using AutoMapper;
using DataAccess.DataAccessRepository.IRepository;
using ECommerceWebApp.Constrains;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWebApp.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        #region fields

        private readonly IUnitOfWork UnitOfWork;
        private readonly IMapper Mapper;

        #endregion

        #region cons

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }

        #endregion

        public async Task<IViewComponentResult> InvokeAsync(int userId)
        {
            var cartItems = HttpContext.Session.GetInt32(SessionKeys.CartItems);
            
            if ( cartItems == null)
            {
                cartItems = await UnitOfWork.CartItems.CountByUserIdAsync(userId);
                HttpContext.Session.SetInt32(SessionKeys.CartItems, cartItems.Value);
            };
            
            return View(model: cartItems);
        }

    }


}
