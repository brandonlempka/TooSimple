using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TooSimple.Managers;
using TooSimple.Models.ActionModels;
using TooSimple.Models.DataModels.Plaid;

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

        public async Task<IActionResult> Index()
        {
            ClaimsPrincipal currentUser = this.User;
            var viewModel = await _dashboardManager.GetDashboardVMAsync(currentUser);

            return View("~/Views/Dashboard/Dashboard.cshtml", viewModel);
        }

        [HttpPost]
        public async Task PlaidLink([FromBody] PublicTokenRM dataModel)
        {
            ClaimsPrincipal currentUser = this.User;

            if (!string.IsNullOrWhiteSpace(dataModel.public_token))
            {
                var access_token = await _dashboardManager.PublicTokenExchangeAsync(dataModel, currentUser);
            }
        }

        public async Task<IActionResult> Accounts()
        {
            ClaimsPrincipal currentUser = this.User;
            var viewModel = await _dashboardManager.GetDashboardAccountsVMAsync(currentUser);
            return View("~/Views/Dashboard/DashboardAccounts.cshtml", viewModel);
        }

        public async Task<IActionResult> LoadAccount(string Id)
        {
            ClaimsPrincipal currentUser = this.User;
            var viewModel = await _dashboardManager.GetIndividualAccountVMAsync(Id, currentUser);
            return View("~/Views/Dashboard/DashboardEditAccount.cshtml", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAccount(DashboardEditAccountAM actionModel)
        {
            var response = await _dashboardManager.UpdateAccountAsync(actionModel);
            return Json(response);
        }

        public async Task<IActionResult> DeleteAccount(string Id)
        {
            var response = await _dashboardManager.DeleteAccountAsync(Id);
            return RedirectToAction("Accounts", response);
        }
    }
}
