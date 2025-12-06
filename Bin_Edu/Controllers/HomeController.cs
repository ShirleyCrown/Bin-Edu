using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.Extensions.Logging;

namespace Bin_Edu.Controllers
{
    public class HomeController : Controller
    {
       
       

        [HttpGet("")]
        public IActionResult GetHomePage()
        {
            
            return View("~/Views/Home/WebPage.cshtml");
        }


    }
}