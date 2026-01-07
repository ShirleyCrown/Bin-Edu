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

            int pageSize = 10;

            int totalItems = await _context.ExerciseSubmissions
                .CountAsync(s => s.CourseExerciseId == exerciseId);

            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var submissions = await _context.ExerciseSubmissions
                .Where(s => s.CourseExerciseId == exerciseId)
                .Select(s => new GetSubmissionResponse
                {
                    Id = s.Id,
                    Score = s.Score,
                    StudentName = s.CourseRegistration.Student.FullName,
                    SubmittedAt = DateOnly.FromDateTime(s.SubmittedAt),
                })
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Json(new
            {
                data = submissions,
                totalPages = totalPages,
                currentPage = page
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

            int courseId = await _context.CourseExercises
                .Where(e => e.Id == exerciseId)
                .Select(e => e.CourseId)
                .FirstOrDefaultAsync();

            var registration = await _context.CourseRegistrations
                .Where(cr => cr.StudentId == userId && cr.CourseId == courseId)
                .FirstOrDefaultAsync();

            if (registration == null)
            {
                return new ApiResponse<object>
                {
                    Message = "Student is not registered for this course",
                    Data = null
                };
            }

            ExerciseSubmission submission = new()
            {
                CourseExerciseId = exerciseId,
                CourseRegistrationId = registration.Id,
                FileName = fileName
            };

            _context.ExerciseSubmissions.Add(submission);
            await _context.SaveChangesAsync();

            return new ApiResponse<object>
            {
                Message = "Submit exercise successfully",
                Data = null
            };
        }

        [HttpGet("admin/dashboard/exercise/api/exercise-submission/{submissionId}/download")]
        [Authorize]
        public IActionResult DownloadMySubmission(int submissionId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var submission = _context.ExerciseSubmissions
                .FirstOrDefault(es => es.Id == submissionId);

            if (submission == null)
                return NotFound("Submission not found");

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ExerciseUploads", submission.FileName);


            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found");

            var contentType = "application/octet-stream";

            return PhysicalFile(
                filePath,
                contentType,
                submission.FileName
            );
        }

        [HttpGet("/api/exercise-submission/{exerciseId}/download")]
        [Authorize]
        public IActionResult DownloadStudentSubmission(int exerciseId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            int courseId = _context.CourseExercises
                    .Where(e => e.Id == exerciseId)
                    .Select(e => e.CourseId)
                    .FirstOrDefault();

            var registration = _context.CourseRegistrations
                .FirstOrDefault(r => r.StudentId == userId && r.CourseId == courseId);

            if (registration == null)
                return Unauthorized();

            var submission = _context.ExerciseSubmissions
                .FirstOrDefault(es =>
                    es.CourseExerciseId == exerciseId &&
                    es.CourseRegistrationId == registration.Id);

            if (submission == null)
                return NotFound("Submission not found");

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ExerciseUploads", submission.FileName);


            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found");

            var contentType = "application/octet-stream";

            return PhysicalFile(
                filePath,
                contentType,
                submission.FileName
            );
        }

        [HttpGet("admin/dashboard/exercise/api/submission/{submissionId}/score")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult GetScore(int submissionId)
        {
            var submission = _context.ExerciseSubmissions
                .FirstOrDefault(es => es.Id == submissionId);

            if (submission == null)
                return NotFound("Submission not found");

            return Json(new
            {
                Score = submission.Score ?? 0
            });
        }

        [HttpPut("admin/dashboard/exercise/api/submission/{submissionId}/update-score")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult UpdateScore(
            int submissionId,
            [FromBody] UpdateScoreRequest request
        )
        {
            var submission = _context.ExerciseSubmissions
                .FirstOrDefault(es => es.Id == submissionId);

            if (submission == null)
                return NotFound("Submission not found");

            submission.Score = request.Score;
            _context.SaveChanges();

            return Json(new
            {
                redirect = $"admin/dashboard/exercise/{submission.CourseExerciseId}/exercise-submission"
            });
        }

    }
}