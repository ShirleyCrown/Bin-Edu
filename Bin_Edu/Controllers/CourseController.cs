using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bin_Edu.Controllers
{
    public class CourseController : Controller
    {
        
        

        [HttpGet("admin/dashboard/course-management")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult GetCourseManagementAdminPage()
        {
            return View("~/Views/CourseManagement/GetCourses/WebPage.cshtml");
        }

    }
}