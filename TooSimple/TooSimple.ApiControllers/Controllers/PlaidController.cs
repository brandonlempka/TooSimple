using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace TooSimple.ApiControllers.Controllers
{
    [ApiController]
    [Route("[controller")]
    public class PlaidController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Test 123";
        }
    }
}
