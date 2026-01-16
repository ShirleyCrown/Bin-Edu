using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Bin_Edu.Infrastructure;
using Bin_Edu.Infrastructure.Api;
using Bin_Edu.Infrastructure.Database.Models;
using Bin_Edu.Controllers.RequestDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bin_Edu.Controllers
{
    public class SubjectController : Controller
    {

        private readonly AppDBContext _context;


        public SubjectController(AppDBContext context)
        {
            _context = context;
        }



        [HttpGet("subjects")]
        public async Task<IActionResult> Index()
        {
            var subjects = await _context.Subjects.ToListAsync();

            return Json(subjects);
        }

        [HttpGet("admin/dashboard/subject-management")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> SubjectManagement()
        {
            return View("~/Views/SubjectManagement/WebPage.cshtml");
        }

        [HttpGet("admin/dashboard/subject-management/get-subjects")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetSubjectsAdminApi()
        {
            var subjects = await _context.Subjects
                .OrderBy(s => s.SubjectName)
                .Select(s => new
                {
                    s.Id,
                    s.SubjectName
                })
                .ToListAsync();

            return Json(new ApiResponse<object>
            {
                Message = "Get subjects successfully",
                Data = subjects
            });
        }


        [HttpPost("admin/dashboard/subject-management/create-subject")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateSubjectAdminApi([FromForm] CreateSubjectAdminRequest requestDto)
        {
            // Validation: not empty
            if (string.IsNullOrWhiteSpace(requestDto.SubjectName))
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Subject name is required.",
                    Message = "Subject name is required."
                });

            // Unique check
            bool exists = await _context.Subjects.AnyAsync(s => s.SubjectName == requestDto.SubjectName);
            if (exists)
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Subject already exists.",
                    Message = "Subject already exists."
                });

            Subject subject = new Subject
            {
                SubjectName = requestDto.SubjectName
            };

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return Json(new ApiResponse<dynamic>
            {
                Data = "admin/dashboard/subject-management",
                Message = "Subject created successfully."
            });
        }


        [HttpGet("admin/dashboard/subject-management/get-subject/{subject_id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetSubjectAdminApi([FromRoute(Name = "subject_id")] int subjectId)
        {
            var subject = await _context.Subjects
                .Select(s => new
                {
                    s.Id,
                    s.SubjectName
                })
                .FirstOrDefaultAsync(s => s.Id == subjectId);

            return Json(new ApiResponse<dynamic>
            {
                Message = "Get subject successfully",
                Data = subject
            });
        }


        [HttpPut("admin/dashboard/subject-management/update-subject/{subject_id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateSubjectAdminApi([
            FromRoute(Name = "subject_id")] int subjectId,
            [FromForm] UpdateSubjectAdminRequest requestDto
        )
        {
            if (string.IsNullOrWhiteSpace(requestDto.UpdateSubjectName))
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Subject name is required.",
                    Message = "Subject name is required."
                });

            bool exists = await _context.Subjects
                .AnyAsync(s => s.Id != subjectId && s.SubjectName == requestDto.UpdateSubjectName);

            if (exists)
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Subject already exists.",
                    Message = "Subject already exists."
                });

            Subject? subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == subjectId);

            if (subject == null)
            {
                return NotFound(new ApiResponse<dynamic>
                {
                    Message = "Subject not found"
                });
            }

            subject.SubjectName = requestDto.UpdateSubjectName;

            await _context.SaveChangesAsync();

            return Json(new ApiResponse<dynamic>
            {
                Data = "admin/dashboard/subject-management",
                Message = "Subject updated successfully."
            });
        }


        [HttpDelete("admin/dashboard/subject-management/delete-subject/{subject_id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteSubjectAdminApi([
            FromRoute(Name = "subject_id")] int subjectId
        )
        {
            var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == subjectId);

            if (subject == null)
            {
                return NotFound(new ApiResponse<dynamic>
                {
                    Message = "Subject not found"
                });
            }

            // prevent deletion when there are courses linked to this subject
            bool hasCourses = await _context.Courses.AnyAsync(c => c.SubjectId == subjectId);
            if (hasCourses)
            {
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Subject has associated courses. Delete or reassign courses first.",
                    Message = "Subject has associated courses."
                });
            }

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            return Json(new ApiResponse<dynamic>
            {
                Data = "admin/dashboard/subject-management",
                Message = "Delete subject successfully"
            });
        }


    }
}