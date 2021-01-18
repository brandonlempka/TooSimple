﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.ViewModels
{
    public class DashboardVM
    {
        public string AccountId { get; set; }
        public int AccountTypeId { get; set; }
        public int PlaidAccountId { get; set; }
        public int Mask { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public decimal? CurrentBalance { get; set; }
        public decimal? AvailableBalance { get; set; }
        public string CurrencyCode { get; set; }
        public string AccessToken { get; set; }
        public IEnumerable<TransactionListVM> Transactions { get; set; }
    }
}
