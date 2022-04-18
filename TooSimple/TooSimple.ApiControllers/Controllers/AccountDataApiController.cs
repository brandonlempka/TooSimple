using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TooSimple.Managers.Managers;
using TooSimple.Poco.Models.ViewModels;

namespace TooSimple.ApiControllers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountDataController : ControllerBase
    {
        private IDashboardManager _dashboardManager;
        public AccountDataController(IDashboardManager dashboardManager)
        {
            _dashboardManager = dashboardManager;
        }

        [HttpGet(Name = "GetAccountBalance")]
        public async Task<ActionResult<DashboardVM>> GetAccountBalance(string userAccountId = "")
        {
            userAccountId = "1d4c76c2-148b-47b5-9a53-c29f3a233c80";

            var results = await _dashboardManager.GetDashboardVMAsync(new ClaimsPrincipal(), userAccountId);
            return results;
        }

        [HttpPost("webhookHandler")]
        public async Task<ActionResult> WebhookHandler([FromBody] JsonElement json)
        {
            _ = await _dashboardManager.UpdatePlaidAccountDataAsync();

            return Ok();
        }
    }
}
