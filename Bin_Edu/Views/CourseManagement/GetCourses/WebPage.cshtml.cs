using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Bin_Edu.Views.CourseManagement.GetCourses
{
    public class WebPage : PageModel
    {
        private readonly ILogger<WebPage> _logger;

        public WebPage(ILogger<WebPage> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}