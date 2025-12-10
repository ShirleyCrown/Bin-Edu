using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Bin_Edu.Controllers.ResponseDto;
using Bin_Edu.Infrastructure;
using Bin_Edu.Infrastructure.Api;
using Bin_Edu.Infrastructure.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bin_Edu.Controllers
{
    public class HomeController : Controller
    {

        private readonly AppDBContext _context;



        public HomeController(
            AppDBContext context
        )
        {
            _context = context;
        }
       
       

        [HttpGet("")]
        public IActionResult GetHomePage()
        {
            
            return View("~/Views/Home/WebPage.cshtml");
        }

        [HttpGet("get-courses")]
        public async Task<IActionResult> HandleGetCourseListApi(
            [FromQuery(Name = "page")] int page 
        )
        {
            
            List<Course> query = await _context.Courses
                .Select(c => new Course
                {
                    Id = c.Id,
                    CourseTitle = c.CourseTitle,
                    CourseDescription = c.CourseDescription,
                    CourseSubject = c.CourseSubject,
                    TeachingTeacherName = c.TeachingTeacherName,
                    CoursePrice = c.CoursePrice,
                    OpeningDate = c.OpeningDate,
                    EndDate = c.EndDate,
                    CourseRegistrations = c.CourseRegistrations
                })
                .Skip(page * 10)
                .Take(10)
                .ToListAsync();

            List<GetCoursesResponse> responseDto = new List<GetCoursesResponse>();
            foreach (var queryData in query)
            {

                DateTime openDt = queryData.OpeningDate.ToDateTime(TimeOnly.MinValue);
                DateTime endDt = queryData.EndDate.ToDateTime(TimeOnly.MinValue);

                // Get difference
                TimeSpan diff = endDt - openDt;

                // Full weeks
                int weeks = diff.Days / 7;

                GetCoursesResponse responseData = new GetCoursesResponse
                {
                    Id = queryData.Id,
                    CourseTitle = queryData.CourseTitle,
                    CourseDescription = queryData.CourseDescription,
                    CourseSubject = queryData.CourseSubject,
                    TeachingTeacherName = queryData.TeachingTeacherName,
                    CoursePrice = queryData.CoursePrice,
                    NumberOfStudents = queryData.CourseRegistrations.Count,
                    WeekDuration = weeks
                };

                responseDto.Add(responseData);
            }

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


        [HttpGet("course-detail/{course_id}")]
        public IActionResult GetCourseDetailPage(
            [FromRoute(Name = "course_id")] int courseId
        )
        {

            ViewBag.CourseId = courseId;
            
            return View("~/Views/CourseDetail/WebPage.cshtml");
        }

        [HttpGet("get-course-detail/{course_id}")]
        public async Task<IActionResult> HandleGetCourseDetail(
            [FromRoute(Name = "course_id")] int courseId
        )
        {

            Course? query = await _context.Courses
                .Where(c => c.Id == courseId)
                .Select(c => new Course
                {
                    Id = c.Id,
                    CourseTitle = c.CourseTitle,
                    CourseDescription = c.CourseDescription,
                    TeachingTeacherName = c.TeachingTeacherName,
                    CoursePrice = c.CoursePrice,
                    CourseSubject = c.CourseSubject,
                    CourseRegistrations = c.CourseRegistrations,
                    CourseTimetables = c.CourseTimetables,
                    OpeningDate = c.OpeningDate,
                    EndDate = c.EndDate
                })
                .FirstOrDefaultAsync();


            DateTime openDt = query.OpeningDate.ToDateTime(TimeOnly.MinValue);
            DateTime endDt = query.EndDate.ToDateTime(TimeOnly.MinValue);

            // Get difference
            TimeSpan diff = endDt - openDt;

            // Full weeks
            int weeks = diff.Days / 7;


            CourseDetail courseDetail = new CourseDetail
            {
                Id = query.Id,
                CourseTitle = query.CourseTitle,
                CourseDescription = query.CourseDescription,
                CourseSubject = query.CourseSubject,
                TeachingTeacherName = query.TeachingTeacherName,
                CoursePrice = query.CoursePrice,
                NumberOfStudents = query.CourseRegistrations.Count,
                WeekDuration = weeks,
                Timetables = query.CourseTimetables
                    .GroupBy(ct => new {ct.DayOfWeek, ct.StartTime, ct.EndTime})
                    .Select(g => new CourseTimetableDetail
                    {
                        DayOfWeek = g.Key.DayOfWeek,
                        StartTime = g.Key.StartTime,
                        EndTime = g.Key.EndTime
                    })
                    .ToList()
            };


            GetCourseDetailResponse responseDto = new GetCourseDetailResponse
            {
                CourseDetail = courseDetail
            };

            
            return Json(new ApiResponse<GetCourseDetailResponse>
            {
                Message = "Get course detail successfully",
                Data = responseDto
            });
        }


    }
}