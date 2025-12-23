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
    public class ExerciseSubmissionController : Controller
    {
        private readonly AppDBContext _context;

        public ExerciseSubmissionController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet("admin/dashboard/exercise/{exerciseId}/exercise-submission")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult Index(int exerciseId)
        {
            return View("~/Views/SubmissionManagement/GetSubmissions/Webpage.cshtml");
        }

        [HttpGet("admin/api/exercise-submission/{exerciseId}/submissions")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetSubmission(
            int exerciseId,
            [FromQuery(Name = "page")] int page
        )
        {
            var exercise = await _context.CourseExercises
                .FirstOrDefaultAsync(e => e.Id == exerciseId);

            if (exercise == null)
            {
                return NotFound(new { message = "Exercise not found" });
            }

            List<GetSubmissionResponse> submissions = await _context.ExerciseSubmissions
                .Where(s => s.CourseExerciseId == exerciseId)
                .Select(s => new GetSubmissionResponse
                {
                    Id = s.Id,
                    StudentName = s.CourseRegistration.Student.FullName,
                    SubmittedAt = DateOnly.FromDateTime(s.SubmittedAt),
                })
                .Skip(page * 10)
                .Take(10)
                .ToListAsync();

            return Json(new ApiResponse<List<GetSubmissionResponse>>
            {
                Data = submissions,
                Message = "Submissions retrieved successfully"
            });
        }
    }
}