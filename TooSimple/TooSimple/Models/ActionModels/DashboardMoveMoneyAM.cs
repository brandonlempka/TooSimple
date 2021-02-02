﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.ActionModels
{
    public class DashboardMoveMoneyAM
    {
        public string UserAccountId { get; set; }
        public string FromAccountId { get; set; }
        public string ToAccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
