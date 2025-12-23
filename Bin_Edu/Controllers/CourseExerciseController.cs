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
    public class CourseExerciseController : Controller
    {
        private readonly AppDBContext _context;


        public CourseExerciseController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet("admin/dashboard/exercise-management/{courseId}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult GetExerciseManagementAdminPage(int courseId)
        {
            return View("~/Views/ExerciseManagement/GetExercises/WebPage.cshtml");
        }

        [HttpGet("admin/dashboard/exercise-management/{courseId}/get-exercises")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetExercisesApi(
            int courseId,
            [FromQuery(Name = "page")] int page)
        {
            List<GetExerciseAdminResponse> responseDto = _context.CourseExercises
                .Where(c => c.CourseId == courseId)
                .Select(c => new GetExerciseAdminResponse
                {
                    Id = c.Id,
                    Name = c.ExerciseName,
                    Description = c.ExerciseDescription,
                    SubmitDeadline = DateOnly.FromDateTime(c.ExerciseSubmitDeadline),
                    CreatedAt = DateOnly.FromDateTime(c.CreatedAt)
                })
                .Skip(page * 10)
                .Take(10)
                .ToList();

            int totalPages = await _context.CourseExercises
                                .Where(c => c.CourseId == courseId).CountAsync();

            totalPages = (int)Math.Ceiling((double)totalPages / 10);

            return Json(new ApiResponse<dynamic>
            {
                Message = "Get List of exercises successfully",
                Data = new
                {
                    Exercises = responseDto,
                    TotalPages = totalPages
                }
            });
        }


        [HttpPost("admin/dashboard/exercise-management/{courseId}/create-exercise")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateExercise(int courseId, [FromForm] CreateCourseExcerciseRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ExerciseName))
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = "Exercise name cannot be empty",
                    Data = null
                });
            }

            if (string.IsNullOrWhiteSpace(request.ExerciseDescription))
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = "Exercise description cannot be empty",
                    Data = null
                });
            }

            if (request.ExerciseSubmitDeadline == new DateOnly(1, 1, 1))
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = "Exercise submit deadline cannot be empty",
                    Data = null
                });
            }

            if (request.ExerciseSubmitDeadline < DateOnly.FromDateTime(DateTime.UtcNow))
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = "Exercise submit deadline must be in the future",
                    Data = null
                });
            }

            var course = await _context.Courses.FindAsync(courseId);

            var courseExercise = await _context.CourseExercises.FirstOrDefaultAsync(c => c.CourseId == courseId && c.ExerciseName == request.ExerciseName);
            if (course == null)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = "Course not found",
                    Data = null
                });
            }

            if (courseExercise != null)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = "Exercise with the same name already exists in this course",
                    Data = null
                });
            }

            var newExercise = new CourseExercise
            {
                CourseId = courseId,
                ExerciseName = request.ExerciseName,
                ExerciseDescription = request.ExerciseDescription,
                ExerciseSubmitDeadline = request.ExerciseSubmitDeadline.ToDateTime(TimeOnly.MinValue),
                CreatedAt = DateTime.UtcNow
            };

            _context.CourseExercises.Add(newExercise);
            await _context.SaveChangesAsync();

            return Json(new ApiResponse<string>
            {
                Data = $"admin/dashboard/exercise-management/{courseId}",
                Message = "Exercise created successfully"
            });
        }


        [HttpGet("admin/dashboard/exercise-management/get-course-exercise/{exerciseId}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetCourseExercise(int exerciseId)
        {
            var exercise = await _context.CourseExercises.FindAsync(exerciseId);

            if (exercise == null)
            {
                return NotFound(new ApiResponse<string>
                {
                    Message = "Exercise not found",
                    Data = null
                });
            }

            var responseDto = new GetCourseExerciseDetailResponse
            {
                Id = exercise.Id,
                ExerciseName = exercise.ExerciseName,
                ExerciseDescription = exercise.ExerciseDescription,
                ExerciseSubmitDeadline = DateOnly.FromDateTime(exercise.ExerciseSubmitDeadline),
            };

            return Json(new ApiResponse<GetCourseExerciseDetailResponse>
            {
                Message = "Get course exercise successfully",
                Data = responseDto
            });
        }

        [HttpPut("admin/dashboard/exercise-management/update-course-exercise/{exerciseId}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateCourseExercise(int exerciseId, [FromForm] UpdateCourseExerciseRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UpdateExerciseName))
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = "Exercise name cannot be empty",
                    Data = null
                });
            }

            if (string.IsNullOrWhiteSpace(request.UpdateExerciseDescription))
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = "Exercise description cannot be empty",
                    Data = null
                });
            }

            if (request.UpdateExerciseSubmitDeadline == new DateOnly(1, 1, 1))
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = "Exercise submit deadline cannot be empty",
                    Data = null
                });
            }

            if (request.UpdateExerciseSubmitDeadline < DateOnly.FromDateTime(DateTime.UtcNow))
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = "Exercise submit deadline must be in the future",
                    Data = null
                });
            }

            var exercise = await _context.CourseExercises.FindAsync(exerciseId);

            if (exercise == null)
            {
                return NotFound(new ApiResponse<string>
                {
                    Message = "Exercise not found",
                    Data = null
                });
            }

            if (exercise.ExerciseName == request.UpdateExerciseName)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = "Exercise name cannot be the same as the current exercise name",
                    Data = null
                });
            }

            exercise.ExerciseName = request.UpdateExerciseName;
            exercise.ExerciseDescription = request.UpdateExerciseDescription;
            exercise.ExerciseSubmitDeadline = request.UpdateExerciseSubmitDeadline.ToDateTime(TimeOnly.MinValue);

            await _context.SaveChangesAsync();

            return Json(new ApiResponse<string>
            {
                Data = $"admin/dashboard/exercise-management/{exercise.CourseId}",
                Message = "Exercise updated successfully"
            });
        }

        [HttpDelete("admin/dashboard/exercise-management/delete-course-exercise/{exerciseId}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteCourseExercise(int exerciseId)
        {
            var exercise = await _context.CourseExercises.FindAsync(exerciseId);

            if (exercise == null)
            {
                return NotFound(new ApiResponse<string>
                {
                    Message = "Exercise not found",
                    Data = null
                });
            }

            _context.CourseExercises.Remove(exercise);
            await _context.SaveChangesAsync();

            return Json(new ApiResponse<string>
            {
                Data = $"admin/dashboard/exercise-management/{exercise.CourseId}",
                Message = "Exercise deleted successfully"
            });
        }




















    }
}