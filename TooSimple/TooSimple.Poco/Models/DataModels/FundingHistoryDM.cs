﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Poco.Models.DataModels
{
    public class FundingHistoryDM
    {
        public string FundingHistoryId { get; set; }
        public string FromAccountId { get; set; }
        public string FromAccountName { get; set; }
        public string ToAccountId { get; set; }
        public string ToAccountName { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransferDate { get; set; }
        public string Note { get; set; }
        public bool AutomatedTransfer { get; set; }
    }
}
