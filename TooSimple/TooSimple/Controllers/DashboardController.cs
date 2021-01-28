﻿using Microsoft.AspNetCore.Authorization;
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
            var currentUser = this.User;
            var viewModel = await _dashboardManager.GetDashboardVMAsync(currentUser);

            return View("~/Views/Dashboard/Dashboard.cshtml", viewModel);
        }

        [HttpPost]
        public async Task PlaidLink([FromBody] PublicTokenRM dataModel)
        {
            var currentUser = this.User;

            if (!string.IsNullOrWhiteSpace(dataModel.public_token))
            {
                var access_token = await _dashboardManager.PublicTokenExchangeAsync(dataModel, currentUser);
            }
        }

        public async Task<IActionResult> LoadTransaction(string Id)
        {
            var currentUser = this.User;
            var viewModel = await _dashboardManager.GetEditTransactionVMAsync(Id, currentUser);

            return View("~/Views/Dashboard/DashboardEditTransaction.cshtml", viewModel);
        }

        public async Task<IActionResult> SaveTransaction(DashboardEditTransactionAM actionModel)
        {
            var response = await _dashboardManager.UpdateTransactionAsync(actionModel);
            return RedirectToAction("Index", response);
        }

        public async Task<IActionResult> Accounts()
        {
            var currentUser = this.User;

            var viewModel = await _dashboardManager.GetDashboardAccountsVMAsync(currentUser);
            return View("~/Views/Dashboard/DashboardAccounts.cshtml", viewModel);
        }

        public async Task<IActionResult> LoadAccount(string Id)
        {
            var currentUser = this.User;

            var viewModel = await _dashboardManager.GetIndividualAccountVMAsync(Id, currentUser);
            return View("~/Views/Dashboard/DashboardEditAccount.cshtml", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAccount(DashboardSaveAccountAM actionModel)
        {
            var response = await _dashboardManager.UpdateAccountAsync(actionModel);
            return RedirectToAction("Accounts", response);
        }

        public async Task<IActionResult> DeleteAccount(string Id)
        {
            var response = await _dashboardManager.DeleteAccountAsync(Id);
            return RedirectToAction("Accounts", response);
        }

        public async Task<IActionResult> Goals()
        {
            var currentUser = this.User;
            var viewModel = await _dashboardManager.GetGoalsVMAsync(currentUser);

            return View("~/Views/Dashboard/DashboardGoals.cshtml", viewModel);
        }

        public async Task<IActionResult> AddEditGoal(string goalId = "")
        {
            var currentUser = this.User;
            var viewModel = await _dashboardManager.GetSaveGoalVMAsync(goalId, currentUser);
            return View("~/Views/Dashboard/DashboardEditGoal.cshtml", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveGoal(DashboardSaveGoalAM actionModel)
        {
            var response = await _dashboardManager.UpdateGoalAsync(actionModel);
            return Json(response);
        }

    }
}
