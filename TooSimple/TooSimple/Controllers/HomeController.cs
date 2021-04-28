using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TooSimple.DataAccessors.Plaid;
using TooSimple.Poco.Models;

namespace TooSimple.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IPlaidDataAccessor _plaidDataAccessor;

        public HomeController(ILogger<HomeController> logger, IPlaidDataAccessor plaidDataAccessor)
        {
            _logger = logger;
            _plaidDataAccessor = plaidDataAccessor;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Dashboard");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
