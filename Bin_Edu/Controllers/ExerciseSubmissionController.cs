using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
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

        [HttpPost("api/exercise-submission/{exerciseId}/submit")]
        [Authorize(Roles = "STUDENT")]
        public async Task<ApiResponse<object>> SubmitExercise(
            int exerciseId,
            [FromForm] SubmitExerciseRequest request
        )
        {
            Console.WriteLine("UPLOADING");
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            Console.WriteLine("File length:" + request.File.Length);
            if (request.File == null || request.File.Length == 0)
            {
                return new ApiResponse<object>
                {
                    Message = "File is empty",
                    Data = null
                };
            }

            if (request.File.Length > 10 * 1024 * 1024)
            {
                return new ApiResponse<object>
                {
                    Message = "File too large",
                    Data = null
                };
            }

            var allowedExt = new[] { ".doc", ".docx", ".pdf" };
            string ext = Path.GetExtension(request.File.FileName).ToLower();
            Console.WriteLine("File length:" + request.File.Length);

            if (!allowedExt.Contains(ext))
            {
                return new ApiResponse<object>
                {
                    Message = "File type is not allow",
                    Data = null
                };
            }

            var fileName = $"{userId}_{exerciseId}{ext}";

            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ExerciseUploads");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            string filePath = Path.Combine(uploadPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await request.File.CopyToAsync(stream);

            return new ApiResponse<object>
            {
                Message = "Submit exercise successfully",
                Data = null
            };
        }
    }
}