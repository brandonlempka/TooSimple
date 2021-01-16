using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TooSimple.Managers;

namespace TooSimple.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private IDashboardManager _dashboardManager;

        public DashboardController(IDashboardManager dashboardManager)
        {
            _dashboardManager = dashboardManager;
        }

        public IActionResult Index()
        {
            ClaimsPrincipal currentUser = this.User;
            var viewModel = _dashboardManager.GetDashboardVMAsync(currentUser);
            return View();
        }
    }
}
