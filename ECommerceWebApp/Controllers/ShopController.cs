using AutoMapper;
using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using ECommerceWebApp.Areas.Admin.DTOs.Item;
using ECommerceWebApp.DTOs.Item;
using ECommerceWebApp.DTOs.Shop;
using ECommerceWebApp.Models.Shop;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWebApp.Controllers
{
    public class ShopController : Controller
    {
        #region fields
        private readonly IUnitOfWork UnitOfWork;
        private readonly IMapper Mapper;
        #endregion

        #region cons
        public ShopController(IUnitOfWork ufw, IMapper mapper)
        {
            UnitOfWork = ufw;
            Mapper = mapper;
        }
        #endregion

        #region actions

        [HttpGet]
        public async Task<IActionResult> Index(int? categoryId)
        {
            var categories = await UnitOfWork.Categories.GetCategoriesHierarchyAsync();
            return View(new ShopIndexViewModel
            {
                CategoryId = categoryId,
                Categories = categories
            });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var item = await UnitOfWork.Items.GetItemDetailsAsync(id);

            await UnitOfWork.Items.UpdateAsync(new Item
            {
                Id = item.Id,
                Views = item.Views + 1
            },new[]
            {
                nameof(Item.Views)
            });

            var categories = await UnitOfWork.Items.GetCategoriesIdsAsync(id);

            var model = Mapper.Map<ShopItemDetailsViewModel>(item);
            model.CategoryId = categories.LastOrDefault();

            var lookUp = new Dictionary<int, Variation>();
            foreach (var option in item.Options)
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

        #endregion

        #region ajax
        public async Task<IEnumerable<GetShopItemsDto>> GetItems(ShopItemsFilterDto filter,int? skip = null, int? take = null)
        {
            var sortBy = filter.SortBy switch
            {
                ShopItemsFilterDto.SortByOptions.PriceAsc=>"price asc",
                ShopItemsFilterDto.SortByOptions.PriceDesc=>"price desc",
                ShopItemsFilterDto.SortByOptions.ViewsAsc => "views asc",
                ShopItemsFilterDto.SortByOptions.ViewsDesc => "views desc",
                _ => null
            };

            var items = await UnitOfWork.Items.GetITemsCardDetailsAsync(filter.CategoryId,sortBy,filter.MinPrice,filter.MaxPrice,skip,take);
            return Mapper.Map<IEnumerable<GetShopItemsDto>>(items);
        }

        public async Task<IEnumerable<GetItemsDto>> SearchItems(string filter,int? skip = null, int? take = null)
        {
            var items = await UnitOfWork.Items.GetITemsCardDetailsByNameAsync(filter,skip, take);
            return Mapper.Map<IEnumerable<GetItemsDto>>(items);
        }
        #endregion
    }
}
