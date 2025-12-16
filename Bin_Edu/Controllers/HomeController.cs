using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Bin_Edu.Controllers.ResponseDto;
using Bin_Edu.Infrastructure;
using Bin_Edu.Infrastructure.Api;
using Bin_Edu.Infrastructure.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Student_Science_Research_Management_UEF.Infrastructure.Mail;

namespace Bin_Edu.Controllers
{
    public class HomeController : Controller
    {

        private readonly AppDBContext _context;
        private readonly IEmailService _emailService;



        public HomeController(
            AppDBContext context,
            IEmailService emailService
        )
        {
            _context = context;
            _emailService = emailService;
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
                .Skip(page * 9)
                .Take(9)
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

            totalPages = (int) Math.Ceiling((double) totalPages / 9);

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

            // Main course detail
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


            // Related courses
            List<Course> queryCourseDetails = await _context.Courses
                .Where(c => 
                    c.Id != courseId && 
                    c.CourseSubject == query.CourseSubject
                )
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
                .Take(3)
                .ToListAsync();

            List<RelatedCourse> relatedCourses = new List<RelatedCourse>();
            foreach (var queryCourseDetail in queryCourseDetails)
            {

                DateTime openingDate = queryCourseDetail.OpeningDate.ToDateTime(TimeOnly.MinValue);
                DateTime endingDate = queryCourseDetail.EndDate.ToDateTime(TimeOnly.MinValue);

                // Get difference
                TimeSpan diffRelatedCourse = endDt - openDt;

                // Full weeks
                int relatedCourseWeeks = diffRelatedCourse.Days / 7;

                RelatedCourse responseData = new RelatedCourse
                {
                    Id = queryCourseDetail.Id,
                    CourseTitle = queryCourseDetail.CourseTitle,
                    CourseDescription = queryCourseDetail.CourseDescription,
                    CourseSubject = queryCourseDetail.CourseSubject,
                    TeachingTeacherName = queryCourseDetail.TeachingTeacherName,
                    CoursePrice = queryCourseDetail.CoursePrice,
                    NumberOfStudents = queryCourseDetail.CourseRegistrations.Count,
                    WeekDuration = weeks
                };

                relatedCourses.Add(responseData);
            }


            GetCourseDetailResponse responseDto = new GetCourseDetailResponse
            {
                CourseDetail = courseDetail,
                RelatedCourses = relatedCourses
            };


            if (User.Identity.IsAuthenticated)
            {                
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                bool isCourseRegistered = await _context.CourseRegistrations
                    .AnyAsync(cr => 
                        cr.CourseId == courseId && 
                        cr.StudentId == userId
                    );

                ViewBag.IsCourseRegistered = isCourseRegistered;
            }

            
            return Json(new ApiResponse<GetCourseDetailResponse>
            {
                Message = "Get course detail successfully",
                Data = responseDto
            });
        }



        [HttpPost("register-course/{course_id}")]
        [Authorize]
        public async Task<IActionResult> HandleRegisterCourse(
            [FromRoute(Name = "course_id")] int courseId
        )
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            Course? query = await _context.Courses
                .Include(c => c.CourseTimetables)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            bool isCourseRegistered = await _context.CourseRegistrations
                .AnyAsync(cr => cr.CourseId == courseId && cr.StudentId == userId);

            if (isCourseRegistered)
            {
                return Redirect("/");
            }

            CourseRegistration courseRegistration = new CourseRegistration
            {
                CourseId = query.Id,
                StudentId = userId,
                RegisteredAt = DateTime.UtcNow
            };

            await _context.CourseRegistrations.AddAsync(courseRegistration);
            await _context.SaveChangesAsync();


            string htmlEmail = @"
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Register Course Email</title>
                </head>
                <body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
                    <table role='presentation' style='width: 100%; border-collapse: collapse;'>
                        <tr>
                            <td style='padding: 40px 0;'>
                                <table role='presentation' style='width: 600px; margin: 0 auto; background-color: #ffffff; border-collapse: collapse; box-shadow: 0 4px 6px rgba(0,0,0,0.1);'>
                                    <!-- Header -->
                                    <tr>
                                        <td style='background-color: #0d6efd; padding: 40px 30px; text-align: center;'>
                                            <h1 style='margin: 0; color: #ffffff; font-size: 28px; font-weight: bold;'>Bin Edu</h1>
                                        </td>
                                    </tr>
                                    
                                    <!-- Main Content -->
                                    <tr>
                                        <td style='padding: 40px 30px;'>
                                            <h2 style='margin: 0 0 20px 0; color: #333333; font-size: 24px;'>Than you for registering our course</h2>
                                            <p style='margin: 0 0 15px 0; color: #666666; font-size: 16px; line-height: 1.6;'>
                                                Thank you for registering our course, this help us earn lots of money from your.
                                            </p>
                                            <p style='margin: 0 0 25px 0; color: #666666; font-size: 16px; line-height: 1.6;'>
                                                To go into your course, please click to a link below.
                                            </p>
                                            
                                            <!-- Button -->
                                            <table role='presentation' style='margin: 0 auto;'>
                                                <tr>
                                                    <td style='text-align: center; padding: 20px 0;'>
                                                        <a href='/my-courses' target='_blank' style='background-color: #0d6efd; color: #ffffff; padding: 14px 40px; text-decoration: none; border-radius: 5px; font-size: 16px; font-weight: bold; display: inline-block;'>View my courses</a>
                                                    </td>
                                                </tr>
                                            </table>
                                            
                                            <p style='margin: 25px 0 0 0; color: #666666; font-size: 16px; line-height: 1.6;'>
                                                If you have any questions, feel free to reply to this email. We're here to help!
                                            </p>
                                        </td>
                                    </tr>
                                    
                                </table>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>
            ";
            
            _emailService.SendEmailAsync(userEmail, "Register course successfully", htmlEmail);


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

            
            return View("~/Views/RegisterCourse/WebPage.cshtml", courseDetail);
        }


        [HttpGet("my-courses")]
        public IActionResult GetMyCoursesPage()
        {
            
            return View("~/Views/MyCourses/WebPage.cshtml");
        }

        [HttpGet("get-my-courses")]
        public async Task<IActionResult> HandleGetMyCourses(
            [FromQuery(Name = "page")] int page 
        )
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<Course> query = await _context.CourseRegistrations
                .Where(cr => cr.StudentId == userId)
                .Select(cr => new Course
                {
                    Id = cr.Course.Id,
                    CourseTitle = cr.Course.CourseTitle,
                    CourseSubject = cr.Course.CourseSubject,
                    TeachingTeacherName = cr.Course.TeachingTeacherName,
                    CourseTimetables = cr.Course.CourseTimetables,
                    OpeningDate = cr.Course.OpeningDate,
                    EndDate = cr.Course.EndDate
                })
                .Skip(page * 9)
                .Take(9)
                .ToListAsync();

            
            List<MyCourses> myCourses = new List<MyCourses>();

            foreach (var queryData in query)
            {
                DateTime openDt = queryData.OpeningDate.ToDateTime(TimeOnly.MinValue);
                DateTime endDt = queryData.EndDate.ToDateTime(TimeOnly.MinValue);

                // Get difference
                TimeSpan diff = endDt - openDt;

                // Full weeks
                int weeks = diff.Days / 7;
                
                MyCourses myCourse = new MyCourses
                {
                    Id = queryData.Id,
                    CourseTitle = queryData.CourseTitle,
                    CourseSubject = queryData.CourseSubject,
                    TeachingTeacherName = queryData.TeachingTeacherName,
                    Timetables = queryData.CourseTimetables
                        .GroupBy(ct => new {ct.DayOfWeek, ct.StartTime, ct.EndTime})
                        .Select(g => new CourseTimetableDetail
                        {
                            DayOfWeek = g.Key.DayOfWeek,
                            StartTime = g.Key.StartTime,
                            EndTime = g.Key.EndTime
                        })
                        .ToList(),
                    WeekDuration = weeks
                };

                myCourses.Add(myCourse);
            }

            int totalPages = await _context.CourseRegistrations
                .Where(cr => cr.StudentId == userId)
                .CountAsync();

            totalPages = (int) Math.Ceiling((double) totalPages / 9);
            

            return Json(new ApiResponse<GetMyCoursesResponse>
            {
                Message = "Get my courses successfully",
                Data = new GetMyCoursesResponse
                {
                    MyCourses = myCourses,
                    TotalPages = totalPages
                }
            });
        }




        [HttpGet("my-courses/{course_id}/timetable")]
        public IActionResult GetCourseTimetablePage(
            [FromRoute(Name = "course_id")] int courseId
        )
        {

            ViewBag.CourseId = courseId;
            
            return View("~/Views/CourseTimetable/WebPage.cshtml");
        }

        [HttpGet("get-my-course-timetable/{course_id}")]
        public async Task<IActionResult> HandleMyCourseTimetable(
            [FromRoute(Name = "course_id")] int courseId,
            [FromQuery(Name = "selected_date")] string selectedDate
        )
        {

            DateOnly parsedSelectedDate = DateOnly.Parse(selectedDate);

            GetMyCourseTimetableResponse? responseDto = await _context.Courses
                .Where(c => 
                    c.Id == courseId
                )
                .Select(c => new GetMyCourseTimetableResponse
                {
                    CourseTitle = c.CourseTitle,
                    TeachingTeacherName = c.TeachingTeacherName,
                    Timetables = c.CourseTimetables
                        .Where(ct => ct.StartDate == parsedSelectedDate)
                        .Select(ct => new CourseTimetableDetail
                        {
                            DayOfWeek = ct.DayOfWeek,
                            StartTime = ct.StartTime,
                            EndTime = ct.EndTime
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();


            return Json(new ApiResponse<GetMyCourseTimetableResponse>
            {
                Message = "Get my course timetable successfully",
                Data = responseDto
            });
        }


    }
}