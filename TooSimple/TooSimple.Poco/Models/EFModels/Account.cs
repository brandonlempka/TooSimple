using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Poco.Models.EFModels
{
    public class Account
    {
        public string AccountId { get; set; }
        public int AccountTypeId { get; set; }
        public string UserAccountId { get; set; }
        public string Mask { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public decimal? CurrentBalance { get; set; }
        public decimal? AvailableBalance { get; set; }
        public decimal? CreditLimit { get; set; }
        public string CurrencyCode { get; set; }
        public string AccessToken { get; set; }
        public bool UseForBudgeting { get; set; }
        public DateTime? LastUpdated { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
        public bool ReLoginRequired { get; set; }
    }
}
