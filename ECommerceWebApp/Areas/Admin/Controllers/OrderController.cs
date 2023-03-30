using AutoMapper;
using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using ECommerceWebApp.Areas.Admin.DTOs.Order;
using ECommerceWebApp.Models.Order;
using Microsoft.AspNetCore.Mvc;
using static DataAccess.Data.Order;

namespace ECommerceWebApp.Areas.Admin.Controllers
{
    public class OrderController:AdminBaseController
    {
        #region fields
        private readonly IUnitOfWork UnitOfWork;
        private readonly IMapper Mapper;
        #endregion

        #region cons
        public OrderController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
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
            var order = await UnitOfWork.Orders.GetOrderDetailsByIdAsync(id);

            return View(Mapper.Map<OrderDetailsViewModel>(order));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id,Statuses? status)
        {
            if(status != null && id != 0)
            {
                if (await UnitOfWork.Orders.UpdateAsync(new Order
                {
                    Id = id,
                    Status = status.Value
                }, new[]
                {
                    nameof(Order.Status)
                }))
                {
                    TempData["success"] = "Status Is Updated Successfully";
                    return RedirectToAction(nameof(Details), new { id = id });
                }
            }

            TempData["danger"] = "Failed To Update";
            return RedirectToAction(nameof(Details), new { id = id });
        }

        #endregion

        #region ajax
        [HttpGet]
        public async Task<IEnumerable<GetOrdersDto>> GetOrders(int? skip, int? take)
        {
            var orders = await UnitOfWork.Orders.GetAllAsync(new[]
            {
                nameof(Order.Id),
                nameof(Order.TimeStamp),
                nameof(Order.Total),
                nameof(Order.Status)
            }, new[]
            {
                nameof(Order.TimeStamp)
            }, skip: skip, take: take);

            return Mapper.Map<IEnumerable<GetOrdersDto>>(orders);
        }

        [HttpGet]
        public async Task<IEnumerable<GetOrdersDto>> SearchOrders(Statuses status,int? skip, int? take)
        {
            var orders = await UnitOfWork.Orders.GetByStatusAsync(status,new[] 
            {
                nameof(Order.Id),
                nameof(Order.TimeStamp),
                nameof(Order.Total),
                nameof(Order.Status)
            },new[]
            {
                nameof(Order.Id)
            }, skip: skip, take: take);

            return Mapper.Map<IEnumerable<GetOrdersDto>>(orders);
        }
        #endregion
    }
}
