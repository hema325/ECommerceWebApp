using AutoMapper;
using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerceWebApp.Areas.Identity.DTOs;
using ECommerceWebApp.Areas.Identity.Models.Account;
using ECommerceWebApp.Constrains;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;

namespace ECommerceWebApp.Areas.Identity.Controllers
{
    [Authorize]
    public class AccountController : IdentityBaseController
    {
        #region fields

        private readonly IUnitOfWork UnitOfWork;
        private readonly IMapper Mapper;
        private readonly IWebHostEnvironment WebHostEnvironment;
        IPasswordHasher<User> PasswordHasher;

        #endregion

        #region cons

        public AccountController(IUnitOfWork ufw, IMapper mapper, IWebHostEnvironment webHostEnvironment, IPasswordHasher<User> passwordHasher)
        {
            UnitOfWork = ufw;
            Mapper = mapper;
            WebHostEnvironment = webHostEnvironment;
            PasswordHasher = passwordHasher;
        }

        #endregion

        #region actions

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await UnitOfWork.Users.FindByIdIncludeAddreesesAsync(userId, new[]
            {
                nameof(DataAccess.Data.User.FirstName),
                nameof(DataAccess.Data.User.LastName),
                nameof(DataAccess.Data.User.Email),
                nameof(DataAccess.Data.User.PhoneNumber),
                nameof(DataAccess.Data.User.ImgUrl)
            });

            var model = Mapper.Map<AccountIndexViewModel>(user);
            model.Addresses = user.Addresses;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AddAddress()
        {
            var countries = await UnitOfWork.Countries.GetAllAsync();
            return View(new AddAddressViewModel
            {
                Countries = countries.Select(country => new SelectListItem { Text = country.Name, Value = country.Id.ToString() })
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddAddress(AddAddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var address = Mapper.Map<Address>(model);
                address.UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if(await UnitOfWork.Addresses.AddAsync(address))
                {
                    TempData["success"] = "Address Is Added Successfully";
                    return RedirectToAction(nameof(Index));
                }
            }

            TempData["danger"] = "Failed To Add";
            return RedirectToAction(nameof(AddAddressViewModel));
        }

        [HttpPost]
        public async Task<IActionResult> ChangeDetails(ChangeDetailsViewModel model,string returnUrl)
        {
            if (ModelState.IsValid)
            {
                User user = Mapper.Map<User>(model);
                user.Id = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (await UnitOfWork.Users.UpdateAsync(user, new[]
                {
                    nameof(DataAccess.Data.User.FirstName),
                    nameof(DataAccess.Data.User.LastName),
                    nameof(DataAccess.Data.User.PhoneNumber)
                }))
                {
                    TempData["success"] = "Details Updated Successfully";
                    if (!string.IsNullOrEmpty(returnUrl))
                        return LocalRedirect(returnUrl);
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
               
            }

            TempData["danger"] = "Failed To Update";
            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);
            return RedirectToAction("Index", "Home", new {area=""});

        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model,string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UnitOfWork.Users.FindByIdAsync(Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)), new[]
                {
                    nameof(DataAccess.Data.User.Id),
                    nameof(DataAccess.Data.User.Password)
                });

                if(PasswordHasher.VerifyHashedPassword(user,user.Password,model.OldPassword) == PasswordVerificationResult.Success)
                {
                    user.Password = PasswordHasher.HashPassword(user, model.NewPassword);

                    if (await UnitOfWork.Users.UpdateAsync(user, new[]
                    {
                        nameof(DataAccess.Data.User.Password)
                    }))
                    {
                        await HttpContext.SignOutAsync();

                        TempData["success"] = "Your Password Changed Successfully";
                        return RedirectToAction("Index", "Home", new { area = "" });
                    }
                }

            }

            TempData["danger"] = "Failed To Change Your Password";
            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);
            return RedirectToAction("Index", "Home", new { area = "" });

        }

        [HttpPost]

        public async Task<IActionResult> ChangeImage(ChangeImageViewModel model,string returnUrl)
        {
            if (ModelState.IsValid && model.Image.ContentType.Contains("image"))
            {
                var user = await UnitOfWork.Users.FindByIdAsync(Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)), new[]
                {
                    nameof(DataAccess.Data.User.Id),
                    nameof(DataAccess.Data.User.ImgUrl)
                });

                if (user != null)
                {
                    //upload new image
                    var fileName = string.Concat(Guid.NewGuid(), Path.GetExtension(model.Image.FileName));
                    var url = Path.Combine(AvailablePaths.UserImages, fileName);
                    var path = Path.Combine(WebHostEnvironment.WebRootPath, url);

                    using var fileStream = System.IO.File.Create(path);
                    model.Image.CopyTo(fileStream);

                    var oldUrl = user.ImgUrl;
                    user.ImgUrl = url;

                    if (await UnitOfWork.Users.UpdateAsync(user, new[]
                    {
                        nameof(DataAccess.Data.User.ImgUrl)
                    }))
                    {
                        //delete old image
                        if ( oldUrl != DefaultImages.User)
                            System.IO.File.Delete(Path.Combine(WebHostEnvironment.WebRootPath, oldUrl));

                        HttpContext.Session.SetString(SessionKeys.UserImgUrl, user.ImgUrl);

                        TempData["success"] = "Image  Changed Successully";
                        if (!string.IsNullOrEmpty(returnUrl))
                            return LocalRedirect(returnUrl);
                        return RedirectToAction("Index", "Home", new { area = "" });
                    }
                    else
                    {
                        System.IO.File.Delete(path);
                    }
                }

            }


            TempData["danger"] = "Failed To Change Your Image";
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        #endregion

        #region ajax

        [HttpGet]
        public async Task<ChangeDetailsViewModel> ChangeDetails()
        {
            var user = await UnitOfWork.Users.FindByEmailAsync(User.Identity.Name, new[]
            {
                nameof(DataAccess.Data.User.FirstName),
                nameof(DataAccess.Data.User.LastName),
                nameof(DataAccess.Data.User.PhoneNumber)
            });

            return Mapper.Map<ChangeDetailsViewModel>(user);
        }

        [HttpGet]
        public async Task<DetailsDto> Details()
        {
            var user = await UnitOfWork.Users.FindByEmailAsync(User.Identity.Name, new[]
            {
                nameof(DataAccess.Data.User.FirstName),
                nameof(DataAccess.Data.User.LastName),
                nameof(DataAccess.Data.User.Email),
                nameof(DataAccess.Data.User.PhoneNumber),
                nameof(DataAccess.Data.User.ImgUrl)
            });

            return Mapper.Map<DetailsDto>(user);
        }

        [HttpPut]
        public async Task<bool> DeleteAddress(int addressId)
        {
            if (await UnitOfWork.Addresses.DeleteByIdAsync(addressId))
                return true;

            return false;
        }

        #endregion

    }
}
