using AutoMapper;
using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using ECommerceWebApp.Areas.Admin.Models.Option;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWebApp.Areas.Admin.Controllers
{
    public class OptionController : AdminBaseController
    {
        #region fields
        private readonly IUnitOfWork UnitOfWork;
        private readonly IMapper Mapper;
        #endregion

        #region cons 
        public OptionController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }

        #endregion

        #region actions

        [HttpGet]
        public IActionResult Add(int variationId)
        {
            return View(new AddOptionViewModel
            {
                VariationId = variationId
            });
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddOptionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var option = Mapper.Map<Option>(model);

                if(await UnitOfWork.Options.AddAsync(option))
                {
                    TempData["Success"] = "Option Is Added Successfully";
                    return RedirectToAction(nameof(Add), new { variationId = model.VariationId });
                }
            }

            TempData["danger"] = "Failed To Add";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id,int variationId)
        {
            var option = await UnitOfWork.Options.FindByIdAsync(id, new[]
            {
                nameof(Option.Id),
                nameof(Option.Value)
            });

            var model = Mapper.Map<EditOptionViewModel>(option);
            model.VariationId = variationId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditOptionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var option = Mapper.Map<Option>(model);

                if (await UnitOfWork.Options.UpdateAsync(option))
                {
                    TempData["Success"] = "Option Is Updated Successfully";
                    return RedirectToAction(nameof(Edit), new { id = model.Id, variationId = model.VariationId });
                }
            }

            TempData["danger"] = "Failed To Update";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id,string returnUrl)
        {
            if (await UnitOfWork.Options.DeleteByIdAsync(id))
                TempData["success"] = "Option Deleted Successfully";
            else
                TempData["danger"] = "Failed To Delete";

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);
            return RedirectToAction("Index", "Variation");
        }
        #endregion
    }
}
