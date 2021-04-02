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
using TooSimple.Models.ResponseModels;
using TooSimple.Models.ViewModels;

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

        public async Task<DashboardVM> UpdateAccounts()
        {
            var currentUser = this.User;
            var responseModel = await _dashboardManager.UpdatePlaidAccountDataAsync(currentUser);
            return responseModel;
        }

        public async Task<IActionResult> GetTransactionTablePage(int page)
        {
            var currentUser = this.User;
            var viewModel = await _dashboardManager.GetTransactionTableVMAsync(currentUser, page);

            return PartialView("~/Views/Dashboard/_TransactionTable.cshtml", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> PlaidLink([FromBody] PublicTokenRM dataModel)
        {
            var currentUser = this.User;
            var response = new StatusRM();

            if (!string.IsNullOrWhiteSpace(dataModel.public_token))
            {
                response = await _dashboardManager.PublicTokenExchangeAsync(dataModel, currentUser);
            }

            return RedirectToAction("Index", response);
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
            var viewModel = await _dashboardManager.GetGoalsVMAsync(currentUser, false);

            return View("~/Views/Dashboard/DashboardGoals.cshtml", viewModel);
        }

        public async Task<IActionResult> AddEditGoal(bool isExpense, string Id = "")
        {
            var currentUser = this.User;
            var viewModel = await _dashboardManager.GetSaveGoalVMAsync(Id, currentUser, isExpense);
            return View("~/Views/Dashboard/DashboardEditGoal.cshtml", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveGoal(DashboardSaveGoalAM actionModel)
        {
            actionModel.ExpenseFlag = false;
            var response = await _dashboardManager.UpdateGoalAsync(actionModel);
            return RedirectToAction("Goals", response);
        }

        public async Task<IActionResult> DeleteGoal(string Id, bool isExpense)
        {
            var response = await _dashboardManager.DeleteGoalAsync(Id);

            if (!isExpense)
            {
                return RedirectToAction("Goals", response);
            }
            else
            {
                return RedirectToAction("Expenses", response);
            }
        }

        public async Task<IActionResult> Expenses()
        {
            var currentUser = this.User;
            var viewModel = await _dashboardManager.GetGoalsVMAsync(currentUser, true);

            return View("~/Views/Dashboard/DashboardExpenses.cshtml", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveExpense(DashboardSaveGoalAM actionModel)
        {
            actionModel.ExpenseFlag = true;
            var response = await _dashboardManager.UpdateGoalAsync(actionModel);
            return RedirectToAction("Expenses", response);
        }

        public async Task<IActionResult> FundingSchedules()
        {
            var currentUser = this.User;
            var viewModel = await _dashboardManager.GetDashboardFundingScheduleListVM(currentUser);

            return View("~/Views/Dashboard/DashboardFundingSchedules.cshtml", viewModel);
        }

        public async Task<IActionResult> AddEditSchedule(string Id = "")
        {
            var currentUser = this.User;
            var viewModel = await _dashboardManager.GetDashboardFundingScheduleVM(Id, currentUser);

            return View("~/Views/Dashboard/DashboardEditSchedule.cshtml", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveFundingSchedule(DashboardSaveFundingScheduleAM actionModel)
        {
            var response = await _dashboardManager.UpdateFundingScheduleAsync(actionModel);

            return RedirectToAction("FundingSchedules", response);
        }

        public async Task<IActionResult> DeleteSchedule(string Id)
        {
            var response = await _dashboardManager.DeleteFundingScheduleAsync(Id);

            return RedirectToAction("FundingSchedules", response);
        }

        public async Task<IActionResult> MoveMoney()
        {
            var currentUser = this.User;
            var viewModel = await _dashboardManager.GetMoveMoneyVMAsync(currentUser);

            return View("~/Views/Dashboard/DashboardMoveMoney.cshtml", viewModel);
        }

        public async Task<IActionResult> SaveMoveMoney(DashboardMoveMoneyAM actionModel)
        {
            var response = await _dashboardManager.SaveMoveMoneyAsync(actionModel);

            return RedirectToAction("Index", response);
        }
    }
}
