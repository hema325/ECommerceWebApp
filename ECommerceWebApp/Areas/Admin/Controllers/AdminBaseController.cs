using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerceWebApp.Constrains;

namespace ECommerceWebApp.Areas.Admin.Controllers
{
    [Area(ECommerceWebApp.Constrains.Areas.Admin)]
    [Authorize(Roles=Roles.Admin)]
    public abstract class AdminBaseController : Controller { }
}
