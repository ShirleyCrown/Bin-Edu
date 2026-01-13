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

        // [HttpGet("admin/dashboard/course-management/get-courses")]
        // [Authorize(Roles = "ADMIN")]
        // public async Task<IActionResult> GetCoursesAdminApi(
        //     [FromQuery(Name = "page")] int page 
        // )
        // {

        //     List<Course> query = await _context.Courses
        //         .Select(c => new Course
        //         {
        //             Id = c.Id,
        //             CourseTitle = c.CourseTitle,
        //             CourseSubject = c.CourseSubject,
        //             TeachingTeacherName = c.TeachingTeacherName,
        //             CoursePrice = c.CoursePrice,
        //             OpeningDate = c.OpeningDate,
        //             EndDate = c.EndDate,
        //             CourseRegistrations = c.CourseRegistrations
        //         })
        //         .Skip(page * 10)
        //         .Take(10)
        //         .ToListAsync();


        //     List<GetCoursesAdminResponse> responseDto = new List<GetCoursesAdminResponse>();
        //     foreach (var queryData in query)
        //     {

        //         DateTime openDt = queryData.OpeningDate.ToDateTime(TimeOnly.MinValue);
        //         DateTime endDt = queryData.EndDate.ToDateTime(TimeOnly.MinValue);

        //         // Get difference
        //         TimeSpan diff = endDt - openDt;

        //         // Full weeks
        //         int weeks = diff.Days / 7;

        //         GetCoursesAdminResponse responseData = new GetCoursesAdminResponse
        //         {
        //             Id = queryData.Id,
        //             CourseTitle = queryData.CourseTitle,
        //             CourseSubject = queryData.CourseSubject,
        //             TeachingTeacherName = queryData.TeachingTeacherName,
        //             CoursePrice = queryData.CoursePrice,
        //             NumberOfStudents = queryData.CourseRegistrations.Count,
        //             WeekDuration = weeks
        //         };

        //         responseDto.Add(responseData);
        //     }


        //     int totalPages = await _context.Courses.CountAsync();

        //     totalPages = (int) Math.Ceiling((double) totalPages / 10);

        //     return Json(new ApiResponse<dynamic>
        //     {
        //         Message = "Get List of courses successfully",
        //         Data = new
        //         {
        //             Courses = responseDto, 
        //             TotalPages = totalPages
        //         }
        //     });
        // }

        [HttpGet("admin/dashboard/course-management/get-courses")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetCoursesAdminApi([FromQuery] CourseFilterRequest req)
        {

            Console.WriteLine("Subject: " + req.Subject ?? "None");
            const int pageSize = 10;

            IQueryable<Course> query = _context.Courses;

            // 🔍 SEARCH
            if (!string.IsNullOrWhiteSpace(req.Keyword))
            {
                query = query.Where(c =>
                    c.CourseTitle.Contains(req.Keyword));
            }

            // 📚 FILTER SUBJECT
            if (!string.IsNullOrWhiteSpace(req.Subject))
            {
                query = query.Where(c => c.Subject.SubjectName == req.Subject);
            }

            // 💰 FILTER PRICE
            if (req.MinPrice.HasValue)
                query = query.Where(c => c.CoursePrice >= req.MinPrice);

            if (req.MaxPrice.HasValue)
                query = query.Where(c => c.CoursePrice <= req.MaxPrice);

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var courses = await query
                .Skip(req.Page * pageSize)
                .Take(pageSize)
                .Select(c => new GetCoursesAdminResponse
                {
                    Id = c.Id,
                    CourseTitle = c.CourseTitle,
                    CourseSubjectId = c.SubjectId,
                    CourseSubject = c.Subject.SubjectName,
                    TeachingTeacherName = c.TeachingTeacherName,
                    CoursePrice = c.CoursePrice,
                    NumberOfStudents = c.CourseRegistrations.Count,
                    WeekDuration = (c.EndDate.ToDateTime(TimeOnly.MinValue)
                                    - c.OpeningDate.ToDateTime(TimeOnly.MinValue)).Days / 7
                })
                .ToListAsync();

            return Json(new ApiResponse<object>
            {
                Message = "Get List of courses successfully",
                Data = new
                {
                    Courses = courses,
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

            // Console.WriteLine("SubjectId: " + requestDto.SubjectId);
            if (requestDto.SubjectId == null)
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

            if (string.IsNullOrWhiteSpace(requestDto.OpeningDate))
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Opening date is required.",
                    Message = "Opening date is required."
                });

            if (string.IsNullOrWhiteSpace(requestDto.EndDate))
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "End date is required.",
                    Message = "End date is required."
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

            // 4. OPENING DATE GEATER THAN END DATE 
            bool openValid = DateOnly.TryParse(requestDto.OpeningDate, out DateOnly openingDate);
            bool endValid = DateOnly.TryParse(requestDto.EndDate, out DateOnly endDate);

            if (openingDate > endDate)
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Opening Date cannot be greater than End Date.",
                    Message = "Opening Date cannot be greater than End Date."
                });


            Course course = new Course
            {
                TeachingTeacherName = requestDto.TeachingTeacherName,
                CourseTitle = requestDto.CourseTitle,
                CourseDescription = requestDto.CourseDescription,
                SubjectId = requestDto.SubjectId.Value,
                CoursePrice = price,
                OpeningDate = openingDate,
                EndDate = endDate
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            if (requestDto.ThumbNail != null && requestDto.ThumbNail.Length > 0)
            {
                string uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "CourseImages"
                );

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string extension = Path.GetExtension(requestDto.ThumbNail.FileName);
                string fileName = $"{course.Id}{extension}";
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await requestDto.ThumbNail.CopyToAsync(stream);
                }

                // save filename to DB
                course.ThumbNail = fileName;
                await _context.SaveChangesAsync();
            }

            return Json(new ApiResponse<dynamic>
            {
                Data = "admin/dashboard/course-management",
                Message = "Course created successfully."
            });

        }


        [HttpGet("admin/dashboard/course-management/get-course/{course_id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetCourseAdminApi(
            [FromRoute(Name = "course_id")] int courseId
        )
        {

            GetCourseAdminResponse? responseDto = await _context.Courses
                .Select(c => new GetCourseAdminResponse
                {
                    Id = c.Id,
                    CourseTitle = c.CourseTitle,
                    CourseDescription = c.CourseDescription,
                    CourseSubjectId = c.SubjectId,
                    CourseSubject = c.Subject.SubjectName,
                    TeachingTeacherName = c.TeachingTeacherName,
                    CoursePrice = c.CoursePrice,
                    NumberOfStudents = c.CourseRegistrations.Count,
                    OpeningDate = c.OpeningDate,
                    EndDate = c.EndDate,
                    ThumbNail = c.ThumbNail
                })
                .FirstOrDefaultAsync(c => c.Id == courseId);


            return Json(new ApiResponse<dynamic>
            {
                Message = "Get Course successfully",
                Data = responseDto
            });
        }

        [HttpPut("admin/dashboard/course-management/update-course/{course_id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateCourseAdminApi(
            [FromRoute(Name = "course_id")] int courseId,
            [FromForm] UpdateCourseAdminRequest requestDto
        )
        {

            // Validation
            // 1. NULL OR EMPTY VALIDATION
            if (string.IsNullOrWhiteSpace(requestDto.UpdateTeachingTeacherName))
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Teaching teacher name is required.",
                    Message = "Teaching teacher name is required."
                });

            if (string.IsNullOrWhiteSpace(requestDto.UpdateCourseTitle))
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Course title is required.",
                    Message = "Course title is required."
                });

            if (string.IsNullOrWhiteSpace(requestDto.UpdateCourseDescription))
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Course description is required.",
                    Message = "Course description is required."
                });

            if (requestDto.UpdateCourseSubject == null)
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Course subject is required.",
                    Message = "Course subject is required."
                });

            if (string.IsNullOrWhiteSpace(requestDto.UpdateCoursePrice))
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Course price is required.",
                    Message = "Course price is required."
                });

            if (string.IsNullOrWhiteSpace(requestDto.UpdateOpeningDate))
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Opening date is required.",
                    Message = "Opening date is required."
                });

            if (string.IsNullOrWhiteSpace(requestDto.UpdateEndDate))
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "End date is required.",
                    Message = "End date is required."
                });

            // 2. PRICE VALIDATION (NOT NEGATIVE)
            if (!long.TryParse(requestDto.UpdateCoursePrice, out long price))
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
                .AnyAsync(x =>
                    x.Id != courseId &&
                    x.CourseTitle == requestDto.UpdateCourseTitle
                );

            if (titleExists)
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Course title already exists.",
                    Message = "Course title already exists."
                });

            // 4. OPENING DATE GEATER THAN END DATE 
            bool openValid = DateOnly.TryParse(requestDto.UpdateOpeningDate, out DateOnly openingDate);
            bool endValid = DateOnly.TryParse(requestDto.UpdateEndDate, out DateOnly endDate);

            if (openingDate > endDate)
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Opening Date cannot be greater than End Date.",
                    Message = "Opening Date cannot be greater than End Date."
                });


            Course course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);

            course.TeachingTeacherName = requestDto.UpdateTeachingTeacherName;
            course.CourseTitle = requestDto.UpdateCourseTitle;
            course.CourseDescription = requestDto.UpdateCourseDescription;
            course.SubjectId = requestDto.UpdateCourseSubject.Value;
            course.CoursePrice = price;
            course.OpeningDate = openingDate;
            course.EndDate = endDate;

            if (requestDto.UpdateThumbNail != null && requestDto.UpdateThumbNail.Length > 0)
            {
                var ext = Path.GetExtension(requestDto.UpdateThumbNail.FileName);
                var fileName = $"{courseId}{ext}";

                var uploadPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "CourseImages"
                );

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await requestDto.UpdateThumbNail.CopyToAsync(stream);

                course.ThumbNail = fileName;
            }

            await _context.SaveChangesAsync();

            return Json(new ApiResponse<dynamic>
            {
                Data = "admin/dashboard/course-management",
                Message = "Course updated successfully."
            });

        }


        [HttpDelete("admin/dashboard/course-management/delete-course/{course_id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteCourseAdminApi(
    [FromRoute(Name = "course_id")] int courseId
)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
            {
                return NotFound(new ApiResponse<dynamic>
                {
                    Message = "Course not found"
                });
            }

            // DELETE THUMBNAIL IMAGE (IF EXISTS)
            if (!string.IsNullOrWhiteSpace(course.ThumbNail))
            {
                var imagePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "CourseImages",
                    course.ThumbNail
                );

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return Json(new ApiResponse<dynamic>
            {
                Message = "Delete course successfully",
                Data = "admin/dashboard/course-management"
            });
        }





        [HttpGet("admin/dashboard/course-management/get-timetables/{course_id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetTimetables(
            [FromRoute(Name = "course_id")] int courseId
        )
        {
            List<GetTimetablesAdminResponse> responseDto = await _context.CourseTimetables
                .Where(ct => ct.CourseId == courseId)
                .GroupBy(ct => new { ct.DayOfWeek, ct.StartTime, ct.EndTime })
                .Select(g => new GetTimetablesAdminResponse
                {
                    DayOfWeek = g.Key.DayOfWeek,
                    StartTime = g.Key.StartTime,
                    EndTime = g.Key.EndTime
                })
                .ToListAsync();


            return Json(new ApiResponse<List<GetTimetablesAdminResponse>>
            {
                Message = "Get course timtables successfully",
                Data = responseDto
            });
        }


        [HttpPost("admin/dashboard/course-management/create-timetable/{course_id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateTimetable(
            [FromRoute(Name = "course_id")] int courseId,
            [FromForm] CreateTimetableAdminRequest requestDto
        )
        {

            // Validation
            // 1. NULL OR EMPTY VALIDATION
            if (string.IsNullOrWhiteSpace(requestDto.DayOfWeek))
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Day Of Week is required.",
                    Message = "Day Of Week is required."
                });

            if (string.IsNullOrWhiteSpace(requestDto.StartTime))
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Start time is required.",
                    Message = "Start time is required."
                });

            if (string.IsNullOrWhiteSpace(requestDto.EndTime))
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "End time is required.",
                    Message = "End time is required."
                });

            TimeOnly startTime = TimeOnly.Parse(requestDto.StartTime);
            TimeOnly endTime = TimeOnly.Parse(requestDto.EndTime);

            // 2. TIMETABLE UNIQUE CHECK (EF CORE)
            bool timetableExisted = await _context.CourseTimetables
                .AnyAsync(ct =>
                    ct.CourseId == courseId &&
                    ct.DayOfWeek == requestDto.DayOfWeek &&
                    ct.StartTime == startTime &&
                    ct.EndTime == endTime
                );

            if (timetableExisted)
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Timetable already exists.",
                    Message = "Timetable already exists."
                });


            Course? course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
            DayOfWeek dayOfWeek =
                requestDto.DayOfWeek == "Monday" ? DayOfWeek.Monday :
                requestDto.DayOfWeek == "Tuesday" ? DayOfWeek.Tuesday :
                requestDto.DayOfWeek == "Wednesday" ? DayOfWeek.Wednesday :
                requestDto.DayOfWeek == "Thursday" ? DayOfWeek.Thursday :
                requestDto.DayOfWeek == "Friday" ? DayOfWeek.Friday :
                requestDto.DayOfWeek == "Saturday" ? DayOfWeek.Saturday :
                DayOfWeek.Sunday;

            List<DateOnly> courseStartDates = this.GetDatesByDayOfWeek(course.OpeningDate, course.EndDate, dayOfWeek);


            List<CourseTimetable> courseTimetables = new List<CourseTimetable>();

            foreach (var courseStartDate in courseStartDates)
            {
                CourseTimetable courseTimetable = new CourseTimetable
                {
                    CourseId = course.Id,
                    DayOfWeek = requestDto.DayOfWeek,
                    StartDate = courseStartDate,
                    StartTime = TimeOnly.Parse(requestDto.StartTime),
                    EndTime = TimeOnly.Parse(requestDto.EndTime),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                courseTimetables.Add(courseTimetable);
            }


            await _context.CourseTimetables.AddRangeAsync(courseTimetables);
            await _context.SaveChangesAsync();


            return Json(new ApiResponse<dynamic>
            {
                Data = "admin/dashboard/course-management",
                Message = "Course timetable created successfully."
            });
        }



        [HttpGet("admin/dashboard/course-management/get-course-sessions/{course_id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetCourseSessions(
            [FromRoute(Name = "course_id")] int courseId
        )
        {
            List<GetCourseSessionsAdminResponse> responseDto = await _context.CourseTimetables
                .Where(ct => ct.CourseId == courseId)
                .Select(ct => new GetCourseSessionsAdminResponse
                {
                    Id = ct.Id,
                    DayOfWeek = ct.DayOfWeek,
                    StartDate = ct.StartDate,
                    StartTime = ct.StartTime,
                    TotalPresent = ct.CourseAttendances.Count(ca => ca.Status == "Present"),
                    TotalAbsent = ct.CourseAttendances.Count(ca => ca.Status == "Absent"),
                    EndTime = ct.EndTime
                })
                .ToListAsync();


            return Json(new ApiResponse<List<GetCourseSessionsAdminResponse>>
            {
                Message = "Get course sessions successfully",
                Data = responseDto
            });
        }


        [HttpGet("admin/dashboard/course-management/course-sessions/{course_id}/get-attendances/{course_timetable_id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetCourseAttendances(
            [FromRoute(Name = "course_id")] int courseId,
            [FromRoute(Name = "course_timetable_id")] int courseTimetableId
        )
        {
            List<GetAttendancesAdminResponse> responseDto = await _context.CourseRegistrations
                .Where(cr => cr.CourseId == courseId)
                .Select(cr => new GetAttendancesAdminResponse
                {
                    StudentId = cr.StudentId,
                    StudentFullName = cr.Student.FullName,
                    AttendanceStatus = cr.CourseAttendances
                        .Where(ca => ca.CourseTimetableId == courseTimetableId)
                        .Select(ca => ca.Status)
                        .FirstOrDefault(),
                    AttendedAt = cr.CourseAttendances
                        .Where(ca => ca.CourseTimetableId == courseTimetableId)
                        .Select(ca => ca.AttendedAt)
                        .FirstOrDefault()
                        .ToString("dd/MM/yyyy")
                })
                .ToListAsync();


            return Json(new ApiResponse<List<GetAttendancesAdminResponse>>
            {
                Message = "Get course attendances successfully",
                Data = responseDto
            });
        }


        [HttpPost("admin/dashboard/course-management/course-sessions/{course_timetable_id}/mark-present/{student_id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> MarkStudentPresent(
            [FromRoute(Name = "course_timetable_id")] int courseTimetableId,
            [FromRoute(Name = "student_id")] string studentId
        )
        {
            CourseAttendance? attendance = await _context.CourseAttendances
                .Include(ca => ca.CourseRegistration)
                .FirstOrDefaultAsync(ca =>
                    ca.CourseTimetableId == courseTimetableId &&
                    ca.CourseRegistration.StudentId == studentId
                );

            if (attendance == null)
            {
                attendance = new CourseAttendance
                {
                    CourseTimetableId = courseTimetableId,
                    CourseRegistrationId = _context.CourseRegistrations
                        .Where(cr => cr.StudentId == studentId)
                        .Select(cr => cr.Id)
                        .FirstOrDefault(),
                    Status = "Present",
                    AttendedAt = DateTime.UtcNow
                };

                _context.CourseAttendances.Add(attendance);
            }
            else
            {
                attendance.Status = "Present";
                attendance.AttendedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Json(new ApiResponse<object>
            {
                Message = "Marked student as present successfully",
                Data = null
            });
        }


        [HttpPost("admin/dashboard/course-management/course-sessions/{course_timetable_id}/mark-absent/{student_id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> MarkStudentAbsent(
           [FromRoute(Name = "course_timetable_id")] int courseTimetableId,
           [FromRoute(Name = "student_id")] string studentId
       )
        {
            CourseAttendance? attendance = await _context.CourseAttendances
                .Include(ca => ca.CourseRegistration)
                .FirstOrDefaultAsync(ca =>
                    ca.CourseTimetableId == courseTimetableId &&
                    ca.CourseRegistration.StudentId == studentId
                );

            if (attendance == null)
            {
                attendance = new CourseAttendance
                {
                    CourseTimetableId = courseTimetableId,
                    CourseRegistrationId = _context.CourseRegistrations
                        .Where(cr => cr.StudentId == studentId)
                        .Select(cr => cr.Id)
                        .FirstOrDefault(),
                    Status = "Absent",
                    AttendedAt = DateTime.UtcNow
                };

                _context.CourseAttendances.Add(attendance);
            }
            else
            {
                attendance.Status = "Absent";
                attendance.AttendedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Json(new ApiResponse<object>
            {
                Message = "Marked student as absent successfully",
                Data = null
            });
        }



        // ========== PRIVATE METHODS ==============
        private List<DateOnly> GetDatesByDayOfWeek(DateOnly start, DateOnly end, DayOfWeek targetDay)
        {
            var result = new List<DateOnly>();

            // Move start forward to the first matching day
            int daysToAdd = ((int)targetDay - (int)start.DayOfWeek + 7) % 7;
            var firstMatch = start.AddDays(daysToAdd);

            // If the first matching day is after end date, return empty
            if (firstMatch > end)
                return result;

            // Add weekly occurrences
            for (var date = firstMatch; date <= end; date = date.AddDays(7))
            {
                result.Add(date);
            }

            return result;
        }

    }
}