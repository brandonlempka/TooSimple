using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TooSimple.Poco.Enum;

namespace TooSimple.Poco.Models.ViewModels
{
    public class DashboardEditAccountVM
    {
        public string AccountId { get; set; }
        public AccountType AccountTypeId { get; set; }
        public string LastUpdated { get; set; }
        public string Mask { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public decimal? CurrentBalance { get; set; }
        public string CurrentBalanceDisplayValue { get; set; }
        public decimal? AvailableBalance { get; set; }
        public string AvailableBalanceDisplayValue { get; set; }
        public string CurrencyCode { get; set; }
        public string PublicToken { get; set; }
        public bool UseForBudgeting { get; set; }
        public IEnumerable<TransactionListVM> Transactions { get; set; }
        public string ErrorMessage { get; set; }
        public bool RelogRequired { get; set; }
    }
}
