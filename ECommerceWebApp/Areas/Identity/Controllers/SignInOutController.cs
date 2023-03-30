using AutoMapper;
using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerceWebApp.Constrains;
using ECommerceWebApp.Services.Email;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Security.Claims;
using ECommerceWebApp.Areas.Identity.Models.SignInOut;
using Microsoft.AspNetCore.Identity;

namespace ECommerceWebApp.Areas.Identity.Controllers
{
    public class SignInOutController : IdentityBaseController
    {
        #region fields

        private readonly IUnitOfWork UnitOfWork;
        private readonly IMapper Mapper;
        private readonly IEmailSender EmailSender;
        private readonly IPasswordHasher<User> PasswordHasher;

        #endregion

        #region cons

        public SignInOutController(IUnitOfWork unitOfWork,
            IMapper mapper,
            IEmailSender emailSender,
            IPasswordHasher<User> passwordHasher)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
            EmailSender = emailSender;
            PasswordHasher = passwordHasher;
        }

        #endregion

        #region actions

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UnitOfWork.Users.FindByEmailIncludeRolesAsync(model.Email);

                if (user != null)
                {
                    //do some checks

                    if (user.IsBlocked)
                    {
                        TempData["danger"] = "You are Blocked";
                        return RedirectToAction("index", "Home", new { area = "" });
                    }

                    if (!user.EmailConfirmed)
                    {
                        TempData["Info"] = "Please Confirm Your Email";
                        return RedirectToAction("index", "Home", new { area = "" });
                    }

                    if (user.LockOutEnd > DateTime.UtcNow)
                    {
                        TempData["danger"] = "Your account Is Locked Please Try Again Later";
                        return RedirectToAction("index", "Home", new { area = "" });
                    }


                    //try to sign in

                    if (user.Email == model.Email && PasswordHasher.VerifyHashedPassword(user,user.Password,model.Password) == PasswordVerificationResult.Success)
                    {
                        user.AccessFailedCount = 0;

                        await UnitOfWork.Users.UpdateAsync(user, new[]
                        {
                        (nameof(DataAccess.Data.User.AccessFailedCount))
                    });

                        await SignIn(user, isPersistent: model.RememberMe);
                        TempData["Success"] = "Signed In Successfully";
                        if (!string.IsNullOrEmpty(returnUrl))
                            return LocalRedirect(returnUrl);
                        return RedirectToAction("Index", "Home", new { area = "" });
                    }


                    //manage lock out settings

                    user.AccessFailedCount += 1;

                    if (user.AccessFailedCount >= LockOutSettings.AccessFailedCountLimit)
                    {
                        user.LockOutEnd = DateTime.UtcNow.AddHours(LockOutSettings.Hours)
                            .AddMinutes(LockOutSettings.Minutes)
                            .AddSeconds(LockOutSettings.Seconds);
                        user.AccessFailedCount = 0;
                    }

                    await UnitOfWork.Users.UpdateAsync(user, new[]
                    {
                        nameof(DataAccess.Data.User.AccessFailedCount),
                        nameof(DataAccess.Data.User.LockOutEnd)
                    });
                }
            }

            TempData["danger"] = "Failed To Sign In Please Try Again";
            return RedirectToAction("index", "Home", new { area = "" });
        }

        [HttpGet]
        public IActionResult ExternalSignIn(string provider,string returnUrl)
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(ExternalSignInCallBack), new { returnUrl = returnUrl })
            }, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalSignInCallBack(string returnUrl)
        {
            var result = await HttpContext.AuthenticateAsync();
            await HttpContext.SignOutAsync();
            if (result.Succeeded)
            {
                var loginName = result.Principal.Identity.AuthenticationType;
                var providerKey = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                var userLogin = await UnitOfWork.UserLogins.FindByLoginNameAndProviderKey(loginName, providerKey);
                if (userLogin != null)
                {
                    await SignIn(await UnitOfWork.Users.FindByIdIncludeRolesAsync(userLogin.UserId, new[]
                    {
                        nameof(DataAccess.Data.User.Id),
                        nameof(DataAccess.Data.User.Email)
                    }));

                    TempData["success"] = "Signed In Successfully";
                    if (String.IsNullOrEmpty(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("index", "home", new { area = "" });
                }

                //if user doesn't have an email we can redirect him to enter an email
                //but i'll skip that it's just a learning site

                if (result.Principal.FindFirstValue(ClaimTypes.Email) != null)
                {
                    //add user
                    var userId = await UnitOfWork.Users.AddAsync<int>(new User
                    {
                        Email = result.Principal.FindFirstValue(ClaimTypes.Email),
                        FirstName = result.Principal.FindFirstValue(ClaimTypes.GivenName),
                        LastName = result.Principal.FindFirstValue(ClaimTypes.Surname),
                        EmailConfirmed = true,
                        LastSeen = DateTime.UtcNow,
                        LockOutEnd = DateTime.UtcNow.AddDays(-1),
                        ImgUrl = DefaultImages.User
                    });

                    //add role
                    var role = await UnitOfWork.Roles.GetRoleByNameAsync(Roles.Client);
                    await UnitOfWork.UserRoles.AddAsync(new UserRole { RoleId = role.Id, UserId = userId });

                    await UnitOfWork.UserLogins.AddAsync(new UserLogin
                    {
                        LoginName = loginName,
                        ProviderKey = providerKey,
                        UserId = userId
                    });

                    await SignIn(await UnitOfWork.Users.FindByIdIncludeRolesAsync(userId, new[]
                    {
                        nameof(DataAccess.Data.User.Id),
                        nameof(DataAccess.Data.User.Email)
                    }));

                    TempData["success"] = "Signed In Successfully";
                    if (String.IsNullOrEmpty(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("index", "home", new { area = "" });
                }

            }
            
            TempData["danger"] = "Failed To Sign In";
            if (String.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("index", "home", new {area=""});
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            HttpContext.Session.Clear();

            await HttpContext.SignOutAsync();
            TempData["Success"] = "Signed Out Successfully";
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = Mapper.Map<User>(model);
                    user.Password = PasswordHasher.HashPassword(user, model.Password);

                    //add user
                    user.Id = await UnitOfWork.Users.AddAsync<int>(user,nameof(DataAccess.Data.User.Id));

                    if (user.Id != 0)
                    {
                        user = await UnitOfWork.Users.FindByEmailAsync(user.Email, new[] { nameof(DataAccess.Data.User.Id), nameof(DataAccess.Data.User.Email) });

                        //add role to the user
                        var role = await UnitOfWork.Roles.GetRoleByNameAsync(Roles.Client);
                        await UnitOfWork.UserRoles.AddAsync(new UserRole { UserId = user.Id, RoleId = role.Id });


                        //generate token
                        var token = new Token
                        {
                            Value = Guid.NewGuid().ToString(),
                            UserId = user.Id,
                            ValidTill = DateTime.UtcNow.AddSeconds(TokenSettings.Seconds).AddMinutes(TokenSettings.Minutes).AddHours(TokenSettings.Hours),
                            Purpose = Token.Purposes.ConfirmEmail
                        };

                        if (await UnitOfWork.Tokens.AddAsync(token))
                        {

                            // send email verification message
                            var url = Url.Action(nameof(VerifyEmail), "SignInOut", new { id = user.Id, tokenValue = token.Value, purpose = Token.Purposes.ConfirmEmail }, Request.Scheme);
                            var mailMessage = new MailMessage
                            {
                                From = new MailAddress("ECommerceWebApp@gmail.com", "ECommerceWebApp"),
                                Subject = "EmailConfirmation",
                                Body = $"please <a href=\"{url}\"> Click Here </a> To Verify Your Account",
                                IsBodyHtml = true
                            };
                            mailMessage.To.Add(new MailAddress(user.Email));

                            await EmailSender.SendMailAsync(mailMessage);
                            TempData["info"] = "We Have Sent a Message To Your Email Please Verify";
                            return RedirectToAction("Index", "Home", new { area = "" });
                        }

                        TempData["success"] = "Email Created Successfully";
                        return RedirectToAction("Index", "Home", new { area = "" });
                    }
                }
                catch
                {
                    //......
                }
            }
            TempData["danger"] = "Failed To Register Please Try Again";
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpGet]
        public async Task<IActionResult> VerifyEmail([Required] int id, [Required] string tokenValue, [Required] Token.Purposes purpose)
        {
            if (ModelState.IsValid)
            {

                var user = await UnitOfWork.Users.FindByIdIncludeRolesAsync(id, new[]
                {
                    nameof(DataAccess.Data.User.Id),
                    nameof(DataAccess.Data.User.Email)
                });

                if (user != null && await TokenValidation(id, tokenValue, purpose) && purpose == Token.Purposes.ConfirmEmail)
                {
                    user.EmailConfirmed = true;
                    if (await UnitOfWork.Users.UpdateAsync(user, new[] { nameof(DataAccess.Data.User.EmailConfirmed) }))
                    {
                        await UnitOfWork.Tokens.DeleteByValueAsync(tokenValue);

                        await SignIn(user);
                        TempData["success"] = "Successfully Sign In";
                        return RedirectToAction("Index", "Home", new { area = "" });
                    }
                }

                //remove token for security purposes
                await UnitOfWork.Tokens.DeleteByValueAsync(tokenValue);
            }

            return NotFound();
        } 

        [HttpPost]
        public async Task<IActionResult> SendVerification(SendVerificationViewModel model)
        {
            if (ModelState.IsValid)
            {  
                //get user
                var user = await UnitOfWork.Users.FindByEmailAsync(model.Email, new[] 
                {
                    nameof(DataAccess.Data.User.Id),
                    nameof(DataAccess.Data.User.Email),
                    nameof(DataAccess.Data.User.EmailConfirmed)
                });

                if (user != null)
                {
                    if (user.EmailConfirmed && model.Purpose == Token.Purposes.ConfirmEmail)
                    {
                        TempData["danger"] = "Failed To Send Your Account Is Verified";
                        return RedirectToAction("Index", "Home", new { area = "" });
                    }

                    //remove old token
                    await UnitOfWork.Tokens.DeleteByUserIdAndPurposeAsync(user.Id, model.Purpose);

                    // create token
                    var token = new Token
                    {
                        Value = Guid.NewGuid().ToString(),
                        Purpose = model.Purpose,
                        ValidTill = DateTime.UtcNow.AddSeconds(TokenSettings.Seconds).AddMinutes(TokenSettings.Minutes).AddHours(TokenSettings.Hours),
                        UserId = user.Id
                    };

                    if(await UnitOfWork.Tokens.AddAsync(token))
                    {
                        //send verification message

                        var url = Url.Action(nameof(RetrieveAccount), "SignInOut", new {id = user.Id ,tokenValue = token.Value ,purpose = token.Purpose},Request.Scheme);

                        if (model.Method == SendVerificationViewModel.Methods.Email)
                        {
                            var mailMessage = new MailMessage
                            {
                                From = new MailAddress("ECommerceWebApp@gmail.com", "ECommerceWebApp"),
                                Subject = "Retrieveing your Accout",
                                Body = $"please <a href=\"{url}\" >Click Here</a> To Retrieve Your Account",
                                IsBodyHtml = true
                            };

                            mailMessage.To.Add(new MailAddress(user.Email));

                            await EmailSender.SendMailAsync(mailMessage);

                            TempData["success"] = "Verification Message Sent Successfully";
                        }

                        if (model.Method == SendVerificationViewModel.Methods.PhoneNumber)
                        {
                            await UnitOfWork.Tokens.DeleteByValueAsync(token.Value);
                            TempData["info"] = "This Feture is Comming Soon Try Another One";
                        }

                        return RedirectToAction("Index", "Home", new {area=""});
                    }
                }
            }

            TempData["danger"] = "Failed To Send Verification Message";
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpGet]
        public async Task<IActionResult> RetrieveAccount([Required] int id, [Required] string tokenValue, [Required] Token.Purposes purpose)
        {
            if (ModelState.IsValid)
            {
                if (await TokenValidation(id, tokenValue, purpose) && purpose == Token.Purposes.ChangePassword)
                {
                    return View(new RetrieveAccountViewModel
                    {
                        Id = id,
                        TokenValue = tokenValue,
                        Purpose = purpose
                    });
                }

                //remove token for security purposes
                await UnitOfWork.Tokens.DeleteByValueAsync(tokenValue);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> RetrieveAccount(RetrieveAccountViewModel model)
        {
            if (ModelState.IsValid) 
            {
                if (await TokenValidation(model.Id, model.TokenValue, model.Purpose) && model.Purpose == Token.Purposes.ChangePassword)
                {
                    // change password

                    var user = new User
                    {
                        Id = model.Id,
                        Password = model.NewPassword
                    };

                    if (await UnitOfWork.Users.UpdateAsync(user, new[]
                    {
                        nameof(DataAccess.Data.User.Password)
                    }))
                    {
                        await UnitOfWork.Tokens.DeleteByValueAsync(model.TokenValue);
                        TempData["success"] = "Your Password Changed Successfully";
                        return RedirectToAction("Index", "Home", new { area = "" });
                    }
                }

                //remove token for security purposes
                await UnitOfWork.Tokens.DeleteByValueAsync(model.TokenValue);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #endregion

        #region methods

        private async Task SignIn(DataAccess.Data.User user,string scheme = Schemes.Default, bool isPersistent = false)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.Email),
            };

            foreach (var role in user.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role.Name));

            var identity = new ClaimsIdentity(claims, scheme);

            var principle = new ClaimsPrincipal(identity);

           await HttpContext.SignInAsync(scheme, principle,
               new AuthenticationProperties 
               {
                   IsPersistent = isPersistent
               });
        }

        private async Task<bool> TokenValidation(int userId, string tokenValue, Token.Purposes purpose)
        {
            var token = await UnitOfWork.Tokens.FindByValueAsync(tokenValue);

            if (token != null)
            {
                //does the user really has this token
                if (token.UserId == userId && token.Purpose == purpose)
                {
                    if (token.ValidTill > DateTime.UtcNow)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

    }
}
