using Assesment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace Assesment.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        //private readonly IMemoryCache _cache;

        //public HomeController(ILogger<HomeController> logger, IMemoryCache cache)
        //{
        //    _logger = logger;
        //    _cache = cache;
        //}

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UserListPage()
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
    }
}
