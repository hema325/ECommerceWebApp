using AutoMapper;
using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using ECommerceWebApp.Areas.Admin.DTOs.Category;
using ECommerceWebApp.Areas.Admin.Models.Category;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Areas.Admin.Controllers
{
    public class CategoryController : AdminBaseController
    {
        #region fields
        private readonly IUnitOfWork UnitOfWork;
        private readonly IMapper Mapper;
        #endregion

        #region cons
        public CategoryController(IUnitOfWork unitOfWork,IMapper mapper)
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

            return View(new AddCategoryViewModel
            {
                Categories = categories.Select(category => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = category.Name, Value = category.Id.ToString() })
            });
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddCategoryViewModel model)
        {

            if (ModelState.IsValid)
            {
                var category = Mapper.Map<Category>(model);
                if (await UnitOfWork.Categories.AddAsync(category))
                {
                    TempData["success"] = "Category is Added Successfully";
                    return RedirectToAction(nameof(Add));
                }
            }

            TempData["danger"] = "Failed To Add Category";
            return RedirectToAction(nameof(Add));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await UnitOfWork.Categories.FindByIdAsync(id);

            var model = Mapper.Map<EditCategoryViewModel>(category);
            model.Categories = await UnitOfWork.Categories.GetAllAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditCategoryViewModel model)
        {
            if (model.ParentId == 0)
                model.ParentId = null;

            if (ModelState.IsValid)
            {
                var category = Mapper.Map<Category>(model);

                if(await UnitOfWork.Categories.UpdateAsync(category))
                {
                    TempData["success"] = "Category Updated Successfully";
                    return RedirectToAction(nameof(Details), new { id = model.Id });
                }

            }

            TempData["danger"] = "Failed To Update";
            return RedirectToAction(nameof(Edit));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var category = await UnitOfWork.Categories.FindByIdIncludeParent(id, new[]
            {
                nameof(Category.Id),
                nameof(Category.Name)
            }, new[]
            {
                nameof(Category.Name)
            }, nameof(Category.Name));

            return View(Mapper.Map<CategoryDetailsViewModel>(category));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            
            if(await UnitOfWork.Categories.DeleteByIdAsync(id)) 
            {
                TempData["success"] = "Category Deleted Successfully";
            
                return RedirectToAction(nameof(Index));
            }
      
            TempData["danger"] = "Failed To Delete";
            return RedirectToAction(nameof(Details), new { id = id });
        }

        #endregion

        #region ajax

        [HttpGet]
        public async Task<IEnumerable<GetCategoriesDto>> GetCategories(int? skip,int? take)
        {
            var categories = await UnitOfWork.Categories.GetAllAsync(new[]
            {
                nameof(Category.Id),
                nameof(Category.Name)
            }, new[]
            {
                nameof(Category.Id)
            }, skip, take);

            return Mapper.Map<IEnumerable<GetCategoriesDto>>(categories); 
        }

        [HttpGet]
        public async Task<IEnumerable<GetCategoriesDto>> SearchCategories(string filter,int? skip,int? take)
        {
            var categories = await UnitOfWork.Categories.GetByNameAsync(filter, new[]
            {
                nameof(Category.Id),
                nameof(Category.Name)
            }, new[]
            {
                nameof(Category.Id)
            }, skip, take);

            return Mapper.Map<IEnumerable<GetCategoriesDto>>(categories);
        }

        #endregion
    }
}
