using AutoMapper;
using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using ECommerceWebApp.Areas.Admin.DTOs.Item;
using ECommerceWebApp.Constrains;
using ECommerceWebApp.Models.ShoppingCart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceWebApp.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        #region fields
        private readonly IUnitOfWork UnitOfWork;
        private readonly IMapper Mapper;
        #endregion

        #region cons
        public ShoppingCartController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }
        #endregion

        #region actions
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cartItems = await UnitOfWork.CartItems.GetCartDetailsAsync(Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            return View(Mapper.Map<IEnumerable<ShoppingCartIndexViewModel>>(cartItems));
        }

        #endregion

        #region ajax

        [HttpPut]
        public async Task<bool> Increase(int cartItemId)
        {
            var cartItem = await UnitOfWork.CartItems.FindByIdAsync(cartItemId, new[]
            {
                nameof(CartItem.Id),
                nameof(CartItem.Quantity)
            });

            cartItem.Quantity += 1;

            if (await UnitOfWork.CartItems.UpdateAsync(cartItem, new[]
            {
                nameof(CartItem.Quantity)
            }))
            {
                
                return true; 
            }

            return false;
        }

        [HttpPut]
        public async Task<bool> Decrease(int cartItemId)
        {
            var cartItem = await UnitOfWork.CartItems.FindByIdAsync(cartItemId, new[]
            {
                nameof(CartItem.Id),
                nameof(CartItem.Quantity)
            });

            cartItem.Quantity -= 1;

            if (cartItem.Quantity >= 1 && await UnitOfWork.CartItems.UpdateAsync(cartItem, new[]
            {
                nameof(CartItem.Quantity)
            }))
            {
                cartItem.Quantity -= 1;
                return true;
            }
            else if (cartItem.Quantity == 0 && await UnitOfWork.CartItems.DeleteByIdAsync(cartItemId))
            {
                return true;
            }

            return false;
        }

        [HttpPost]
        public async Task<object> AddToCart(int itemId, int quantity)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (await UnitOfWork.CartItems.IsExist(userId, itemId))
                return new { isSucceeded = false, msg = "This Item Already Exist" };

            var item = await UnitOfWork.Items.FindByIdAsync(itemId, new[]
            {
                nameof(Item.Quantity)
            });

            if (item.Quantity >= quantity)
            {

                var cartItem = new CartItem
                {
                    UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                    ItemId = itemId,
                    Quantity = quantity
                };

                if (await UnitOfWork.CartItems.AddAsync(cartItem))
                {
                    HttpContext.Session.SetInt32(SessionKeys.CartItems, HttpContext.Session.GetInt32(SessionKeys.CartItems).Value + 1);
                    return new { isSucceeded = true, msg = "item is Added Succeessfully" };
                }
             

                return new { isSucceeded = false, mag = "Failed To Add An Item" };
            }


            return new { isSucceeded = false, msg = "Quantity You Request Is Not Available" };
        }

        [HttpPut]
        public async Task<object> Delete(int cartItemId)
        {
            if (await UnitOfWork.CartItems.DeleteByIdAsync(cartItemId))
            {
                HttpContext.Session.SetInt32(SessionKeys.CartItems, HttpContext.Session.GetInt32(SessionKeys.CartItems).Value - 1);
                return new { isSucceeded = true, msg = "item Deleted Successfully" };
            }

            return new { isSucceeded = false, msg = "Failed To Delete" };
        }

        #endregion
    }
}
