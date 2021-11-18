using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Poco.Models.ViewModels
{
    public class DashboardAccountsListVM
    {
        public string AccountId { get; set; }
        public decimal? CurrentBalance { get; set; }
        public decimal? AvailableBalance { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string LastUpdatedDisplayValue { get; set; }
        public string UserAccountId { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string CurrencyCode { get; set; }
        public string AccessToken { get; set; }
        public bool ReLoginRequired { get; set; }
    }
}
