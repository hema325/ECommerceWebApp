using AutoMapper;
using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using ECommerceWebApp.Constrains;
using ECommerceWebApp.DTOs.Order;
using ECommerceWebApp.Models.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace ECommerceWebApp.Controllers
{
    [Authorize]
    public class OrderController:Controller
    {
        #region fields 
        private readonly IUnitOfWork UnitOfWork;
        private readonly IMapper Mapper;
        #endregion

        #region cons
        public OrderController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }
        #endregion

        #region actions
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var orders = await UnitOfWork.Orders.GetByUserIdAsync(Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            
            return View(Mapper.Map<IEnumerable<OrderIndexViewModel>>(orders));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var orders = await UnitOfWork.Orders.GetOrderDetailsByIdAsync(id);
            var model = Mapper.Map<OrderDetailsViewModel>(orders);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> PlaceOrder(int addressId)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var cartItems = await UnitOfWork.CartItems.GetCartDetailsAsync(userId);

            var order = new Order
            {
                TimeStamp = DateTime.Now,
                Status = Order.Statuses.UnPaid,
                Total = cartItems.Sum(cartItem =>
                {
                    var price = cartItem.Item.Price;
                    
                    if (cartItem.Item.Product.Discount != null)
                        price -= cartItem.Item.Price * cartItem.Item.Product.Discount.Value / 100;

                    price *= cartItem.Quantity;

                    return price;
                }),
                AddressId = addressId,
                UserId = userId
            };

            var orderId = await UnitOfWork.Orders.AddAsync<int>(order);
            if (orderId != 0)
            {
                var orderItems = cartItems.Select(cartItem =>
                {
                    var orderItem = new OrderItem
                    {
                        ItemId = cartItem.Item.Id,
                        OrderId = orderId,
                        Price = cartItem.Item.Price,
                        Quantity = cartItem.Quantity
                    };

                    if(cartItem.Item.Product.Discount != null)
                        orderItem.Price -= cartItem.Item.Price * cartItem.Item.Product.Discount.Value / 100;

                    return orderItem;
                });

                if(await UnitOfWork.OrderItems.AddAsync(orderItems))
                {
                    await UnitOfWork.CartItems.DeleteByUserIdAsync(userId);
                    HttpContext.Session.SetInt32(SessionKeys.CartItems, 0);
                    TempData["success"] = "Order Is Added Successfully";
                    return RedirectToAction(nameof(Pay), new { orderId = orderId });
                }

                await UnitOfWork.Orders.DeleteByIdAsync(orderId);
            }

            TempData["danger"] = "Failed To Add";
            return RedirectToAction("Index","ShoppingCart");
        }

        [HttpGet]
        public async Task<IActionResult> Pay(int orderId)
        {
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = Url.Action(nameof(SuccessPay), "Order", new {orderId = orderId},Request.Scheme),
                CancelUrl = Url.Action("Index", "Home", new { },Request.Scheme),
            };

            foreach(var orderItem in await UnitOfWork.OrderItems.GetByOrderIdIncludeProductAsync(orderId))
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = orderItem.Item.Product.Name,
                        },
                        UnitAmount = (long?) orderItem.Price * 100,
                    },
                    Quantity = orderItem.Quantity,
                });
            }

            var service = new SessionService();
            var session = service.Create(options);

            await UnitOfWork.Orders.UpdateAsync(new Order
            {
                Id = orderId,
                SessionId = session.Id,
                PaymentIntentId = session.PaymentIntentId
            }, new[]
            {
                nameof(Order.SessionId),
                nameof(Order.PaymentIntentId)
            });

            return Redirect(session.Url);
        }

        [HttpGet]
        public async Task<IActionResult> SuccessPay(int orderId)
        {
            var order = await UnitOfWork.Orders.FindByIdAsync(orderId, new[]
            {
                nameof(Order.Id),
                nameof(Order.SessionId)
            });

            var service = new SessionService();
            var session = await service.GetAsync(order.SessionId);

            if(session.PaymentStatus.ToLower() == "paid")
            {
                order.Status = Order.Statuses.Pending;

                await UnitOfWork.Orders.UpdateAsync(order, new[]
                {
                    nameof(Order.Status)
                });

                TempData["success"] = "Order Is Paid Successfully";
                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }

        #endregion

        #region ajax
        [HttpGet]
        public async Task<IEnumerable<GetAddressesDto>> GetUserAddresses()
        {
            var addresses = await UnitOfWork.Addresses.GetByUserId(Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)));

            return Mapper.Map<IEnumerable<GetAddressesDto>>(addresses);
        }

        [HttpPut]
        public async Task<bool> Cancel(int orderId)
        {
            var order = await UnitOfWork.Orders.FindByIdAsync(orderId, new[]
            {
                nameof(Order.Id),
                nameof(Order.Status),
                nameof(Order.PaymentIntentId)
            });

            if (order.Status != Order.Statuses.UnPaid) 
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = order.PaymentIntentId
                };

                var service = new RefundService();
                var refund = service.CreateAsync(options);

                order.Status = Order.Statuses.Canceled;

                await UnitOfWork.Orders.UpdateAsync(order, new[]
                {
                    nameof(Order.Status)
                });

                return true;
            }

            return false;
        }

        [HttpPut]
        public async Task<bool> Delete(int orderId)
        {

            if (await UnitOfWork.Orders.DeleteByIdAsync(orderId))
                return true;

            return false;
        }
        #endregion
    }
}
