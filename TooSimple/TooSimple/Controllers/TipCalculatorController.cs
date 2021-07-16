using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TooSimple.Controllers
{
    public class TipCalculatorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
