using AutoMapper;
using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using ECommerceWebApp.Areas.Admin.DTOs.Country;
using ECommerceWebApp.Areas.Admin.Models.Country;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWebApp.Areas.Admin.Controllers
{
    public class CountryController : AdminBaseController
    {
        #region fields
        private readonly IUnitOfWork UnitOfWork;
        private readonly IMapper Mapper;
        #endregion

        #region cons
        public CountryController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }
        #endregion

        #region actions
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
        public async Task<IActionResult> Add(AddCountryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var country = Mapper.Map<Country>(model);
                if (await UnitOfWork.Countries.AddAsync(country))
                {
                    TempData["success"] = "Country Is Added Successfully";
                    return RedirectToAction(nameof(Add));
                }
            }

            TempData["danger"] = "Failed To Add";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var country = await UnitOfWork.Countries.FindByIdAsync(id);
            return View(Mapper.Map<EditCountryViewModel>(country));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditCountryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var country = Mapper.Map<Country>(model);
                if (await UnitOfWork.Countries.UpdateAsync(country))
                {
                    TempData["success"] = "Country Is Updated Successfully";
                    return RedirectToAction(nameof(Edit));
                }
            }

            TempData["danger"] = "Failed To Update";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (await UnitOfWork.Countries.DeleteByIdAsync(id))
                TempData["success"] = "Country Is Deleted Successfully";
            else
                TempData["danger"] = "Failed To Delete";

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region ajax
        [HttpGet]
        public async Task<IEnumerable<GetCountriesDto>> GetCountries(int? skip ,int? take)
        {
            var countries = await UnitOfWork.Countries.GetAllAsync(orderByAttrs: new[]
            {
                nameof(Country.Id)
            }, skip: skip, take: take);

            return Mapper.Map<IEnumerable<GetCountriesDto>>(countries);
        }

        [HttpGet]
        public async Task<IEnumerable<GetCountriesDto>> SearchCountries(string filter,int? skip, int? take)
        {
            var countries = await UnitOfWork.Countries.GetByNameAsync(filter,orderByAttrs: new[]
            {
                nameof(Country.Id)
            }, skip: skip, take: take);

            return Mapper.Map<IEnumerable<GetCountriesDto>>(countries);
        }
        #endregion
    }
}
