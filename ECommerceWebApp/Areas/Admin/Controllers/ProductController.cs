using AutoMapper;
using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using ECommerceWebApp.Areas.Admin.DTOs.Product;
using ECommerceWebApp.Areas.Admin.Models.Item;
using ECommerceWebApp.Areas.Admin.Models.Product;
using ECommerceWebApp.Constrains;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerceWebApp.Areas.Admin.Controllers
{
    public class ProductController : AdminBaseController
    {
        #region fields 
        private readonly IUnitOfWork UnitOfWork;
        private readonly IMapper Mapper;
        private readonly IWebHostEnvironment WebHostEnvironment;
        #endregion

        #region cons
        public ProductController(IUnitOfWork unitOfWork,IMapper mapper,IWebHostEnvironment webHostEnvironment)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
            WebHostEnvironment = webHostEnvironment;
        }
        #endregion

        #region actions

        [HttpGet]
        public  IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AddProductDiscount(int productId)
        {
            var discounts = await UnitOfWork.Discounts.GetAllAsync();
            return View(new AddProductDiscountViewModel
            {
                ProductId = productId, 
                Discounts = discounts.Select(discount=>new SelectListItem 
                {
                    Value= discount.Id.ToString(),
                    Text = $"Discount {discount.Value}% From {discount.Start} To {discount.End}"
                })
            });
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var categories = await UnitOfWork.Categories.GetAllAsync();
            var discounts = await UnitOfWork.Discounts.GetAllAsync();

            return View(new AddProductViewModel
            {
                Discounts = discounts.Select(discount => new SelectListItem { Text = $"{discount.Value}% from {discount.Start} to {discount.End}", Value = discount.Id.ToString() }),
                Categories = categories.Select(category => new SelectListItem { Text = category.Name, Value = category.Id.ToString() })
            });
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var product = Mapper.Map<Product>(model);

                if(await UnitOfWork.Products.AddAsync(product))
                {
                    TempData["success"] = "Product Added Successfully";
                    return RedirectToAction(nameof(Add));
                }

            }

            TempData["danger"] = "Failed To Add Product";
            return RedirectToAction(nameof(Add));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await UnitOfWork.Products.FindByIdIncludeAllAsync(id);
            return View(Mapper.Map<ProductDetailsViewModel>(product));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await UnitOfWork.Products.FindByIdAsync(id);

            var categories = await UnitOfWork.Categories.GetAllAsync(new[]
            {
                nameof(Category.Id),
                nameof(Category.Name)
            });

            var discounts = await UnitOfWork.Discounts.GetAllAsync();


            var model = Mapper.Map<EditProductViewModel>(product);
            model.Categories = categories;
            model.Discounts = discounts;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditProductViewModel model)
        {
            if (model.DiscountId == 0)
                model.DiscountId = null;

            if (ModelState.IsValid) 
            {
                var product = Mapper.Map<Product>(model);

                if (await UnitOfWork.Products.UpdateAsync(product, new[]
                {
                    nameof(Product.Name),
                    nameof(Product.Description),
                    nameof(Product.CategoryId),
                    nameof(Product.DiscountId)
                }))
                {
                    TempData["success"] = "Product Updated Successfully Data";
                    return RedirectToAction(nameof(Details), new { id = model.Id });
                }

            }

            TempData["danger"] = "Failed To Update Data";
            return RedirectToAction(nameof(Edit), new { id = model.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {

            if (await UnitOfWork.Products.DeleteByIdAsync(id))
            {
                TempData["success"] = "Product Deleted Successfully";
                return RedirectToAction(nameof(Index));
            }

            TempData["danger"] = "Failed To Delete Product";
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region ajax

        [HttpGet]
        public async Task<IEnumerable<GetProductsDto>> GetProducts(string filter, int? skip = null, int? take = null)
        {
            var products = await UnitOfWork.Products.GetAllIncludeCategoryAsync(new[]
            {
                nameof(Product.Id),
                nameof(Product.Name)
            }, new[]
            {
                nameof(Category.Name)
            }, skip, take, nameof(Category.Name));
            return Mapper.Map<IEnumerable<GetProductsDto>>(products);
        }

        [HttpGet]
        public async Task<IEnumerable<GetProductsDto>> SearchProducts(string filter, int? skip = null, int? take = null)
        {
            var products = await UnitOfWork.Products.GetByNameIncludeCategoryAsync(filter, new[]
            {
                nameof(Product.Id),
                nameof(Product.Name)
            }, new[]
            {
                nameof(Category.Name)
            }, skip, take, nameof(Category.Name));

            return Mapper.Map<IEnumerable<GetProductsDto>>(products);
        }

        #endregion
    }
}
