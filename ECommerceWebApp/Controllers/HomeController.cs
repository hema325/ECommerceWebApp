using DataAccess.DataAccessRepository.IRepository;
using Microsoft.AspNetCore.Mvc;
using ECommerceWebApp.Models;
using System.Diagnostics;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
namespace ECommerceWebApp.Controllers
{
    public class HomeController : Controller
    {
        #region fields
        private readonly IUnitOfWork UnitOfWork;
        private readonly IMapper Mapper;
        #endregion

        #region cons
        public HomeController(IUnitOfWork ufw, IMapper mapper)
        {
            UnitOfWork = ufw;
            Mapper = mapper;
        }
        #endregion

        #region actions
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion
    }
}