using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Bin_Edu.Controllers.RequestDto;
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
        public async Task<IActionResult> GetCoursesAdminApi(
            [FromQuery(Name = "page")] int page 
        )
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
                .Skip(page * 10)
                .Take(10)
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


        [HttpPost("admin/dashboard/course-management/create-course")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateCourseAdminApi(
            [FromForm] CreateCourseAdminRequest requestDto
        )
        {

            // Validation
            // 1. NULL OR EMPTY VALIDATION
            if (string.IsNullOrWhiteSpace(requestDto.TeachingTeacherName))
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Teaching teacher name is required.",
                    Message = "Teaching teacher name is required."
                });

            if (string.IsNullOrWhiteSpace(requestDto.CourseTitle))
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Course title is required.",
                    Message = "Course title is required."
                });

            if (string.IsNullOrWhiteSpace(requestDto.CourseDescription))
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Course description is required.",
                    Message = "Course description is required."
                });

            if (string.IsNullOrWhiteSpace(requestDto.CourseSubject))
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Course subject is required.",
                    Message = "Course subject is required."
                });

            if (string.IsNullOrWhiteSpace(requestDto.CoursePrice))
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Course price is required.",
                    Message = "Course price is required."
                });

            // 2. PRICE VALIDATION (NOT NEGATIVE)
            if (!long.TryParse(requestDto.CoursePrice, out long price))
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Course price must be a valid number.",
                    Message = "Course price must be a valid number."
                });

            if (price < 0)
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Course price cannot be negative.",
                    Message = "Course price cannot be negative."
                });

            // 3. COURSE TITLE UNIQUE CHECK (EF CORE)
            bool titleExists = await _context.Courses
                .AnyAsync(x => x.CourseTitle == requestDto.CourseTitle);

            if (titleExists)
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Course title already exists.",
                    Message = "Course title already exists."
                });


            Course course = new Course
            {
                TeachingTeacherName = requestDto.TeachingTeacherName,
                CourseTitle = requestDto.CourseTitle,
                CourseDescription = requestDto.CourseDescription,
                CourseSubject = requestDto.CourseSubject,
                CoursePrice = price
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return Json(new ApiResponse<dynamic>
            {
                Data = "admin/dashboard/course-management",
                Message = "Course created successfully."
            });

        }

    }
}