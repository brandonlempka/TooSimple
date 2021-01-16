using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.EFModels
{
    public class Account
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string AccountId { get; set; }
        public int AccountTypeId { get; set; }
        public int PlaidAccountId { get; set; }
        public int Mask { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public decimal? CurrentBalance { get; set; }
        public decimal? AvailableBalance { get; set; }
        public string CurrencyCode { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
