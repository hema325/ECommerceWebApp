using AutoMapper;
using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using ECommerceWebApp.Areas.Admin.DTOs.Variation;
using ECommerceWebApp.Areas.Admin.Models.Variation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerceWebApp.Areas.Admin.Controllers
{
    public class VariationController : AdminBaseController
    {
        #region fields
        private readonly IUnitOfWork UnitOfWork;
        private readonly IMapper Mapper;
        #endregion

        #region cons
        public VariationController(IUnitOfWork unitOfWork,IMapper mapper)
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
        public async Task<IActionResult> Add()
        {
            var categories = await UnitOfWork.Categories.GetAllAsync(new[]
            {
                nameof(Category.Id),
                nameof(Category.Name)
            });

            return View(new AddVariationViewModel
            {
                Categories = categories.Select(category => new SelectListItem { Text = category.Name, Value = category.Id.ToString() })
            });
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddVariationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var variation = Mapper.Map<Variation>(model);

                var variationId = await UnitOfWork.Variations.AddAsync<int>(variation);
                if (variationId != 0)
                {
                    TempData["success"] = "Variation Is Added Successfully";
                    return RedirectToAction(nameof(Add), "Option", new { variationId = variationId});
                }
            }

            TempData["danger"] = "Failed To Add Variation";
            return RedirectToAction(nameof(Add));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var variation = await UnitOfWork.Variations.FindByIdAsync(id);
            var model = Mapper.Map<EditVariationViewModel>(variation);
            model.Categories = await UnitOfWork.Categories.GetAllAsync(new[]
            {
                nameof(Category.Id),
                nameof(Category.Name)
            });

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditVariationViewModel model)
        {
            if (model.CategoryId == 0)
                model.CategoryId = null;

            if (ModelState.IsValid)
            {
                var varitaiton = Mapper.Map<Variation>(model);

                if (await UnitOfWork.Variations.UpdateAsync(varitaiton))
                {
                    TempData["success"] = "Variation Updated Successfully";
                    return RedirectToAction(nameof(Details), new { id = model.Id });
                }
            }

            TempData["danger"] = "Failed To Update";
            return RedirectToAction(nameof(Edit));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var variations = await UnitOfWork.Variations.GetByIdIncludeCategoryThenOptionAsync(id, new[]
            {
               nameof(Variation.Id),
               nameof(Variation.Name)
            }, new[]
            {
                nameof(Category.Id),
                nameof(Category.Name)
            }, new[]
            {
                nameof(Option.Id),
                nameof(Option.Value)
            });

            return View(Mapper.Map<VariationDetailsViewModel>(variations));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if(await UnitOfWork.Variations.DeleteByIdAsync(id))
            {
                TempData["success"] = "Variation Deleted Successfully";
                return RedirectToAction(nameof(Index));
            }

            TempData["danger"] = "Failed To Delete";
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region ajax

        [HttpGet]
        public async Task<IEnumerable<GetVariationsDto>> GetVariations(int? skip = null, int? take = null)
        {
            var variations = await UnitOfWork.Variations.GetAllIncludeCategoryAsync(new[]
            {
                nameof(Variation.Id),
                nameof(Variation.Name),
            }, new[]
            {
                nameof(Category.Name)
            }, skip, take, nameof(Category.Name));

            return Mapper.Map<IEnumerable<GetVariationsDto>>(variations);
        }

        [HttpGet]
        public async Task<IEnumerable<GetVariationsDto>> SearchVariations(string filter,int? skip = null, int? take = null)
        {
            var variations = await UnitOfWork.Variations.GetByNameIncludeCategoryAsync(filter,new[]
            {
                nameof(Variation.Id),
                nameof(Variation.Name),
            }, new[]
            {
                nameof(Category.Name)
            }, skip, take, nameof(Category.Name));

            return Mapper.Map<IEnumerable<GetVariationsDto>>(variations);
        }

        #endregion
    }
}
