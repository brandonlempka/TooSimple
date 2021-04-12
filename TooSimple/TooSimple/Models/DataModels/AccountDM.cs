using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.DataModels
{
    public class AccountDM
    {
        public string AccountId { get; set; }
        public int AccountTypeId { get; set; }
        public string UserAccountId { get; set; }
        public string Mask { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public decimal? CurrentBalance { get; set; }
        public decimal? AvailableBalance { get; set; }
        public string CurrencyCode { get; set; }
        public string AccessToken { get; set; }
        public bool UseForBudgeting { get; set; }
        public DateTime? LastUpdated { get; set; }
        public IEnumerable<TransactionDM> Transactions { get; set; }
        public bool ReLoginRequired { get; set; }
    }
}
