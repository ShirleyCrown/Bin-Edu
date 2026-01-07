using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Bin_Edu.Controllers.ResponseDto;
using Bin_Edu.Infrastructure;
using Bin_Edu.Infrastructure.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bin_Edu.Controllers
{
    public class DashboardController : Controller
    {
        private AppDBContext _context;
        public DashboardController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet("admin/dashboard")]
        public IActionResult GetDashboardPage()
        {
            return View("~/Views/AdminDashboard/Webpage.cshtml");
        }

        [HttpGet("admin/api/dashboard/stats")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetDashboardStats([FromQuery] int year)
        {
            int totalCourses = await _context.Courses.CountAsync();

            int totalStudents = await _context.CourseRegistrations
                .Select(r => r.StudentId)
                .Distinct()
                .CountAsync();

            var monthlyRevenue = await _context.CourseRegistrations
                .Where(r => r.RegisteredAt.Year == year)
                .GroupBy(r => r.RegisteredAt.Month)
                .Select(g => new MonthlyRevenueDto
                {
                    Month = g.Key,
                    Revenue = g.Sum(x => x.Course.CoursePrice)
                })
                .ToListAsync();

            // Fill missing months
            var fullMonths = Enumerable.Range(1, 12)
                .Select(m => new MonthlyRevenueDto
                {
                    Month = m,
                    Revenue = monthlyRevenue.FirstOrDefault(x => x.Month == m)?.Revenue ?? 0
                })
                .ToList();

            return Json(new ApiResponse<DashboardStatsResponse>
            {
                Data = new DashboardStatsResponse
                {
                    TotalCourses = totalCourses,
                    TotalStudents = totalStudents,
                    MonthlyRevenue = fullMonths
                },
                Message = "Dashboard statistics loaded"
            });
        }



    }
}