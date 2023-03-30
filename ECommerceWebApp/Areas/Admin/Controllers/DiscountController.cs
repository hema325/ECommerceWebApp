using AutoMapper;
using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using ECommerceWebApp.Areas.Admin.DTOs.Discount;
using ECommerceWebApp.Areas.Admin.Models.Discount;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerceWebApp.Areas.Admin.Controllers
{
    public class DiscountController : AdminBaseController
    {
        #region fields
        private readonly IUnitOfWork UnitOfWork;
        private readonly IMapper Mapper;
        #endregion

        #region cons
        public DiscountController(IUnitOfWork unitOfWork,IMapper mapper)
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
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddDiscountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var discount = Mapper.Map<Discount>(model);

                if(await UnitOfWork.Discounts.AddAsync(discount))
                {
                    TempData["success"] = "Discount Is Added Successfully";
                    return RedirectToAction(nameof(Add));
                }
            }

            TempData["danger"] = "Failed To Add";
            return RedirectToAction(nameof(Add));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var discount = await UnitOfWork.Discounts.FindByIdAsync(id);
            return View(Mapper.Map<DiscountDetailsViewModel>(discount));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var discount = await UnitOfWork.Discounts.FindByIdAsync(id);
            return View(Mapper.Map<EditDiscountViewModel>(discount));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditDiscountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var discount = Mapper.Map<Discount>(model);

                if (await UnitOfWork.Discounts.UpdateAsync(discount))
                {
                    TempData["success"] = "Discount Is Updated Successfully";
                    return RedirectToAction(nameof(Details), new { id = model.Id });
                }
            }

            TempData["danger"] = "Failed To Update";
            return RedirectToAction(nameof(Add));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if(await UnitOfWork.Discounts.DeleteByIdAsync(id))
            {
                TempData["success"] = "Discount Is Deleted Successfully";
                return RedirectToAction(nameof(Add));
            }

            TempData["danger"] = "Failed To Delete";
            return RedirectToAction(nameof(Details));
        }

        #endregion

        #region ajax

        [HttpGet]
        public async Task<IEnumerable<GetDiscountsDto>> GetDiscounts(int? skip = null, int? take = null)
        {
            var discounts = await UnitOfWork.Discounts.GetAllAsync(skip: skip, take: take);
            return Mapper.Map<IEnumerable<GetDiscountsDto>>(discounts);
        }

        #endregion
    }
}
