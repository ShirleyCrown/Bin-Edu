using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Bin_Edu.Infrastructure.Api;
using Microsoft.AspNetCore.Authorization;
using Bin_Edu.Controllers.ResponseDto;
using Bin_Edu.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Bin_Edu.Infrastructure.Database.Models;
using Bin_Edu.Controllers.RequestDto;
using Student_Science_Research_Management_UEF.Infrastructure.Mail;
using Hangfire;
using Student_Science_Research_Management_UEF.Utilities;

namespace Bin_Edu.Controllers
{
    public class StudentController : Controller
    {

        private readonly AppDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public StudentController(AppDBContext context, UserManager<AppUser> userManager, IEmailService emailService, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;
        }

        [HttpGet("admin/dashboard/student-management")]
        public IActionResult GetStudentManagementAdminPage()
        {
            return View("~/Views/StudentManagement/GetStudents/WebPage.cshtml");
        }

        [HttpGet("admin/dashboard/student-management/get-students")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetStudentsAdminApi(
            [FromQuery(Name = "page")] int page
        )
        {

            List<GetStudentsAdminResponse> responseDto = (await _userManager.GetUsersInRoleAsync("STUDENT"))
                .Select(c => new GetStudentsAdminResponse
                {
                    Id = c.Id,
                    FullName = c.FullName,
                    Email = c.Email,
                    Dob = c.Dob,
                    PhoneNumber = c.PhoneNumber,
                    Grade = c.Grade,
                    School = c.School
                })
                .Skip(page * 10)
                .Take(10)
                .ToList();

            int totalPages = (await _userManager.GetUsersInRoleAsync("STUDENT")).Count;

            totalPages = (int)Math.Ceiling((double)totalPages / 10);

            return Json(new ApiResponse<dynamic>
            {
                Message = "Get List of students successfully",
                Data = new
                {
                    Students = responseDto,
                    TotalPages = totalPages
                }
            });
        }

        [HttpPost("admin/dashboard/student-management/create-student")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateStudentAdminApi(
            [FromForm] CreateStudentAdminRequest requestDto
        )
        {

            // Validation
            // 1. NULL OR EMPTY VALIDATION
            if (string.IsNullOrWhiteSpace(requestDto.FullName))
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Full name is required.",
                    Message = "Full name is required."
                });

            if (string.IsNullOrWhiteSpace(requestDto.Email))
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Email is required.",
                    Message = "Email is required."
                });

            if (string.IsNullOrWhiteSpace(requestDto.Dob.ToString()))
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Date of birth is required.",
                    Message = "Date of birth is required."
                });

            if (string.IsNullOrWhiteSpace(requestDto.Grade))
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Grade is required.",
                    Message = "Grade is required."
                });

            if (string.IsNullOrWhiteSpace(requestDto.School))
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "School is required.",
                    Message = "School is required."
                });


            // 2. EMAIL UNIQUE CHECK (EF CORE)
            bool emailExists = await _context.Users
                .AnyAsync(x => x.Email == requestDto.Email);

            if (emailExists)
                return BadRequest(new ApiResponse<dynamic>
                {
                    Data = "Email already exists.",
                    Message = "Course title already exists."
                });


            AppUser student = new AppUser
            {
                UserName = requestDto.Email,
                FullName = requestDto.FullName,
                Email = requestDto.Email,
                Dob = requestDto.Dob,
                PhoneNumber = requestDto.PhoneNumber,
                Grade = requestDto.Grade,
                School = requestDto.School
            };

            string generatedPassword = PasswordGenerator.Generate();

            await _userManager.CreateAsync(student, generatedPassword);
            await _userManager.AddToRoleAsync(student, "STUDENT");

            string subject = "Account Information - BinEdu System";
            string baseWebUrl = _configuration["BaseWebUrl"];
            string loginUrl = $"{baseWebUrl}/login";

            string body = $@"
                Hello <b>{student.FullName}</b>,<br><br>
                Your account has been successfully created.<br><br>

                <b>Login Email:</b> {student.Email}<br>
                <b>Password:</b> {generatedPassword}<br><br>

                <a href='{loginUrl}'
                style='background:#d10000;color:white;padding:10px 14px;border-radius:6px;text-decoration:none;'>
                Login Now
                </a><br><br>

                Please change your password after your first login.
            ";

            BackgroundJob.Enqueue<IEmailService>(email => email.SendEmailAsync(student.Email, subject, body));

            return Json(new ApiResponse<dynamic>
            {
                Data = "admin/dashboard/student-management",
                Message = "Student created successfully."
            });

        }

        [HttpDelete("admin/dashboard/student-management/delete-student/{student_id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteStudentAdminApi(
            [FromRoute(Name = "student_id")] string studentId
        )
        {

            var student = await _userManager.FindByIdAsync(studentId);

            await _userManager.DeleteAsync(student);

            return Json(
                new ApiResponse<dynamic>
                {
                    Message = "Delete student successfully",
                    Data = "admin/dashboard/student-management"
                }
            );
        }

        [HttpPost("admin/dashboard/student-management/regen-student-password/{student_id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> RegenStudentPasswordAdminApi(
            [FromRoute(Name = "student_id")] string studentId
        )
        {

            var student = await _userManager.FindByIdAsync(studentId);
            string generatedPassword = PasswordGenerator.Generate();
            var token = await _userManager.GeneratePasswordResetTokenAsync(student);
            await _userManager.ResetPasswordAsync(student, token, generatedPassword);

            string subject = "Your Password Has Been Reset - BinEdu System";
            string baseWebUrl = _configuration["BaseWebUrl"];
            string loginUrl = $"{baseWebUrl}/login";

            string body = $@"
                Hello <b>{student.FullName}</b>,<br><br>
                Your account has been reset.<br><br>

                <b>Login Email:</b> {student.Email}<br>
                <b>New Password:</b> {generatedPassword}<br><br>

                <a href='{loginUrl}'
                style='background:#d10000;color:white;padding:10px 14px;border-radius:6px;text-decoration:none;'>
                Login Now
                </a><br><br>

                Please change your password after your first login.
            ";

            BackgroundJob.Enqueue<IEmailService>(email => email.SendEmailAsync(student.Email, subject, body));

            return Json(
                new ApiResponse<dynamic>
                {
                    Message = "Delete student successfully",
                    Data = "admin/dashboard/student-management"
                }
            );
        }





        // --------------- SUPPORTED METHODS ---------------

    }
}