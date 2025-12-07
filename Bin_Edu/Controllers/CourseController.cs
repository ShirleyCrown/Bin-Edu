using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Bin_Edu.Controllers.ResponseDto;
using Bin_Edu.Infrastructure;
using Bin_Edu.Infrastructure.Api;
using Bin_Edu.Infrastructure.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bin_Edu.Controllers
{
    public class CourseController : Controller
    {

        private readonly AppDBContext _context;



        public CourseController(
            AppDBContext context
        )
        {
            _context = context;
        }
        
        

        [HttpGet("admin/dashboard/course-management")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult GetCourseManagementAdminPage()
        {
            return View("~/Views/CourseManagement/GetCourses/WebPage.cshtml");
        }

        [HttpGet("admin/dashboard/course-management/get-courses")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetCoursesAdminApi()
        {

            List<GetCoursesAdminResponse> responseDto = await _context.Courses
                .Select(c => new GetCoursesAdminResponse
                {
                    Id = c.Id,
                    CourseTitle = c.CourseTitle,
                    CourseSubject = c.CourseSubject,
                    TeachingTeacherName = c.TeachingTeacherName,
                    CoursePrice = c.CoursePrice,
                    NumberOfStudents = c.CourseRegistrations.Count
                })
                .ToListAsync();

            int totalPages = await _context.Courses.CountAsync();

            totalPages = (int) Math.Ceiling((double) totalPages / 10);

            return Json(new ApiResponse<dynamic>
            {
                Message = "Get List of courses successfully",
                Data = new
                {
                    Courses = responseDto, 
                    TotalPages = totalPages
                }
            });
        }

    }
}