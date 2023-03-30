using AutoMapper;
using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using ECommerceWebApp.Areas.Admin.DTOs.Item;
using ECommerceWebApp.Areas.Admin.Models.Item;
using ECommerceWebApp.Constrains;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerceWebApp.Areas.Admin.Controllers
{
    public class ItemController : AdminBaseController
    {
        #region fields
        private readonly IUnitOfWork UnitOfWork;
        private readonly IMapper Mapper;
        private readonly IWebHostEnvironment WebHostEnvironment;
        #endregion

        #region cons
        public ItemController(IUnitOfWork unitOfWork,IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
            WebHostEnvironment = webHostEnvironment;
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
            var products = await UnitOfWork.Products.GetAllAsync(new[]
            {
                nameof(Product.Id),
                nameof(Product.Name),
                nameof(Product.CategoryId)
            });

            return View(new AddItemViewModel
            {
                Products = products.Select(product => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = product.Name, Value = product.Id.ToString() })
            });
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddItemViewModel model)
        {
            if (model.ProductId == 0)
                model.ProductId = null;

            if (ModelState.IsValid && model.Image.Name.ToLower() == "image")
            {
                var item = Mapper.Map<Item>(model);

                var imgName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);
                var imgUrl = Path.Combine(AvailablePaths.ItemImages,imgName);
                var path = Path.Combine(WebHostEnvironment.WebRootPath, imgUrl);

                using (var fileStram = System.IO.File.Create(path))
                {
                    model.Image.CopyTo(fileStram);
                }

                item.ImgUrl = imgUrl;

                var itemId = await UnitOfWork.Items.AddAsync<int>(item);
                if (itemId != 0) 
                {
                    TempData["success"] = "Item Is Added Successfully";
                    return RedirectToAction(nameof(AddItemOptions), new { itemId = itemId });
                }

                System.IO.File.Delete(path);

            }

            TempData["danger"] = "Failed To Add This Item";
            return RedirectToAction(nameof(Add));
        }

        [HttpGet]
        public async Task<IActionResult> AddItemOptions(int itemId,string returnUrl)
        {
            var categoriesIds = await UnitOfWork.Items.GetCategoriesIdsAsync(itemId);
            var variations = await UnitOfWork.Variations.GetVariationsIncludeOptionsByCategoriesIds(categoriesIds, new[]
            {
                nameof(Variation.Id),
                nameof(Variation.Name)
            },
            new[] {
                nameof(Option.Id),
                nameof(Option.Value)
            });

            return View(new AddItemOptionsViewModel
            {
                ItemId = itemId,
                ReturnUrl = returnUrl,
                Variations = variations
                });
        }

        [HttpPost]
        public async Task<IActionResult> AddItemOptions(AddItemOptionsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var itemOptions = model.OptionsIDs.Select(optionId => new ItemOption { ItemId = model.ItemId, OptionId = optionId });
                if (await UnitOfWork.ItemOptions.Add(itemOptions))
                {
                    TempData["success"] = "Options Is/are added Successfully";
                    if (!string.IsNullOrEmpty(model.ReturnUrl))
                        return LocalRedirect(model.ReturnUrl);
                    return RedirectToAction(nameof(Add));
                }
            }

            TempData["danger"] = "Failed To Add Options";
            if (!string.IsNullOrEmpty(model.ReturnUrl))
                return LocalRedirect(model.ReturnUrl);
            return RedirectToAction(nameof(Add));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var items = await UnitOfWork.Items.GetItemDetailsAsync(id);

            var model = Mapper.Map<ItemDetailsViewModel>(items);

            var lookUp = new Dictionary<int,Variation>();
            foreach(var option in items.Options)
            {
                if (option == null)
                    continue;

                if (!lookUp.ContainsKey(option.Variation.Id))
                    lookUp.Add(option.Variation.Id, option.Variation);

                if (lookUp[option.Variation.Id].Options == null)
                    lookUp[option.Variation.Id].Options = new List<Option>();

                lookUp[option.Variation.Id].Options.Add(option);
            }

            model.Variations = lookUp.Values.ToList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await UnitOfWork.Items.FindByIdAsync(id, new[]
            {
                nameof(Item.ImgUrl)
            });

            if(item != null)
            {
                var path = Path.Combine(WebHostEnvironment.WebRootPath, item.ImgUrl);
                System.IO.File.Delete(path);

                await UnitOfWork.Items.DeleteByIdAsync(id);

                TempData["success"] = "Item Deleted Successfully";
                return RedirectToAction(nameof(Index));
            }

            TempData["danger"] = "Failed To Delete This Item";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteOption(int itemId,int optionId)
        {
            if (await UnitOfWork.ItemOptions.DeleteAsync(itemId, optionId))
                TempData["success"] = "Option Deleted Successfully";
            else
                TempData["danger"] = "Failed To Delete";

            return RedirectToAction(nameof(Details), new { id = itemId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await UnitOfWork.Items.FindByIdAsync(id);

            var model = Mapper.Map<EditItemViewModel>(item);
            model.Products = await UnitOfWork.Products.GetAllAsync(new[]
            {
                nameof(Product.Id),
                nameof(Product.Name),
            });

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var item = Mapper.Map<Item>(model);

                var path = "";
                if (model.Image != null && model.Image.Name.ToLower() == "image")
                {
                    var imgName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);
                    var imgUrl = Path.Combine(AvailablePaths.ItemImages, imgName);
                    path = Path.Combine(WebHostEnvironment.WebRootPath, imgUrl);

                    using (var fileStram = System.IO.File.Create(path))
                    {
                        model.Image.CopyTo(fileStram);

                        item.ImgUrl = imgUrl;
                    }
                }
                    
                if (await UnitOfWork.Items.UpdateAsync(item))
                    {
                        if (model.Image != null && model.Image.Name.ToLower() == "image")
                        {
                            var oldPath = Path.Combine(WebHostEnvironment.WebRootPath, model.ImgUrl);
                            System.IO.File.Delete(oldPath);
                        }

                        TempData["success"] = "Item Updated Successfully";
                        return RedirectToAction(nameof(Details), new { id = model.Id });
                    }

                    System.IO.File.Delete(path);
                }

            TempData["danger"] = "Failed To Update";
            return RedirectToAction(nameof(Edit), new { id = model.Id });
        }

        #endregion

        #region ajax

        [HttpGet]
        public async Task<IEnumerable<GetItemsDto>> GetItems(int? skip = null, int? take = null)
        {
            var items = await UnitOfWork.Items.GetAllIncludeProductAsync(new[]
            {
                nameof(Item.Id),
                nameof(Item.Quantity),
                nameof(Item.Price),
                nameof(Item.ImgUrl),
                nameof(Item.Quantity)
            }, new[]
            {
                nameof(Product.Name)
            }, skip, take, nameof(Product.Name));

            return Mapper.Map<IEnumerable<GetItemsDto>>(items);
        }

        [HttpGet]
        public async Task<IEnumerable<GetItemsDto>> SearchItems(string filter,int? skip = null, int? take = null)
        {
            var items = await UnitOfWork.Items.GetByNameIncludeProductAsync(filter,new[]
            {
                nameof(Item.Id),
                nameof(Item.Quantity),
                nameof(Item.Price),
                nameof(Item.ImgUrl),
                nameof(Item.Quantity)
            }, new[]
            {
                nameof(Product.Name)
            }, skip, take, nameof(Product.Name));

            return Mapper.Map<IEnumerable<GetItemsDto>>(items);
        }


        #endregion
    }
}
