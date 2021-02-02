using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TooSimple.DataAccessors;
using TooSimple.Models;
using TooSimple.Models.DataModels.Plaid;
using TooSimple.Models.ViewModels;

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
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
