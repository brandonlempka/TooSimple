﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Models.ViewModels
{
    public class DashboardExpenseListVM
    {
        public IEnumerable<DashboardExpenseVM> Expenses { get; set; }
    }
}
