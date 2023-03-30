using AutoMapper;
using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using ECommerceWebApp.DTOs.Review;
using ECommerceWebApp.Models.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceWebApp.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        #region fields 
        private readonly IUnitOfWork UnitOfWork;
        private readonly IMapper Mapper;
        #endregion

        #region cons
        public ReviewController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }
        #endregion

        #region action
        [HttpPost]
        public async Task<IActionResult> Add(AddReviewViewModel model,string returnUrl)
        {
            var review = Mapper.Map<Review>(model);
            review.UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (await UnitOfWork.Reviews.AddAsync(review))
                TempData["success"] = "Review Is Added Successfully";
            else
                TempData["danger"] = "Failed To Add";

            if(!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);
            return RedirectToAction("index", "Order");
        }
        #endregion
        #region ajax
        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<GetItemReviewsDto>> GetItemReviews(int itemId,int?skip,int?take)
        {
            var reviews = await UnitOfWork.Reviews.GetByItemIdAsync(itemId, skip, take);
            return Mapper.Map<IEnumerable<GetItemReviewsDto>>(reviews);
        }
        #endregion

    }
}
