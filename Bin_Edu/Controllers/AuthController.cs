using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Bin_Edu.Controllers.RequestDto;
using Bin_Edu.Infrastructure;
using Bin_Edu.Infrastructure.Api;
using Bin_Edu.Infrastructure.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bin_Edu.Controllers
{
    public class AuthController : Controller
    {
        
        private readonly AppDBContext _context;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;





        public AuthController(
            AppDBContext context,
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager
        )
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }




        [HttpGet("admin/login")]
        public IActionResult GetLoginAdminPage()
        {
            
            return View("~/Views/Auth/LoginAdmin/WebPage.cshtml");
        }

        [HttpGet("student/register")]
        public IActionResult GetRegisterStudentPage()
        {
            
            return View("~/Views/Auth/RegisterStudent/WebPage.cshtml");
        }

        [HttpGet("student/login")]
        public IActionResult GetLoginStudentPage()
        {
            
            return View("~/Views/Auth/LoginStudent/WebPage.cshtml");
        }


        [HttpPost("admin/login")]
        public async Task<IActionResult> HandleLoginAdmin(
            [FromForm] LoginAdminRequest requestDto
        )
        {


            // VALIDATION

            if (requestDto.Username == null || requestDto.Password == null)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = "Invalid Admin credentials",
                    Data = "Invalid Admin credentials"
                });
            }

            var user = await _userManager.FindByNameAsync(requestDto.Username);

            if (user == null)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = "Admin account not existed",
                    Data = "Admin account not existed"
                });
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                requestDto.Password,
                isPersistent: true,    // remember me
                lockoutOnFailure: false
            );


            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = "Invalid Admin credentials",
                    Data = "Invalid Admin credentials"
                });
            }
            
            
            return Json(new ApiResponse<string>
            {
                Message = "Dang nhap thanh cong",
                Data = "admin/dashboard/course-management"
            });
        }

        [HttpPost("student/register")]
        public async Task<IActionResult> HandleRegisterStudent(
            [FromForm] RegisterStudentRequest requestDto
        )
        {


            // VALIDATION
            if (
                requestDto.FullName == null || 
                requestDto.Email == null || 
                requestDto.PhoneNumber == null || 
                requestDto.School == null || 
                requestDto.Grade == null || 
                requestDto.Dob == null || 
                requestDto.Password == null || 
                requestDto.ConfirmPassword == null
            )
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = "You must fill all fields",
                    Data = "You must fill all fields"
                });
            }

            bool isEmailExisted = await _context.Users.AnyAsync(u => u.Email == requestDto.Email);
            if (isEmailExisted)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = "Email already existed",
                    Data = "Email already existed"
                });
            }

            bool isPhoneNumberExisted = await _context.Users.AnyAsync(u => u.PhoneNumber == requestDto.PhoneNumber);
            if (isPhoneNumberExisted)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = "Phone number already existed",
                    Data = "Phone number already existed"
                });
            }

            if (!requestDto.Password.Equals(requestDto.ConfirmPassword))
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = "Confirm password not matches",
                    Data = "Confirm password not matches"
                });
            }


            var user = new AppUser
            {
                FullName = requestDto.FullName,
                UserName = requestDto.Email,
                Email = requestDto.Email,
                PhoneNumber = requestDto.PhoneNumber,
                Grade = requestDto.Grade,
                School = requestDto.School,
                EmailConfirmed = true,
                Dob = DateOnly.Parse(requestDto.Dob)
            };


            var createSucceed = await _userManager.CreateAsync(user, requestDto.Password);

            if (createSucceed.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "STUDENT");
            }
            else
            {
                 foreach (var error in createSucceed.Errors)
                {
                    Console.WriteLine($"Code: {error.Code} | Description: {error.Description}");
                }
            }
            
            return Json(new ApiResponse<string>
            {
                Message = "Dang ky tai khoan thanh cong",
                Data = "student/login"
            });
        }
        

        [HttpPost("student/login")]
        public async Task<IActionResult> HandleLoginStudent(
            [FromForm] LoginStudentRequest requestDto
        )
        {


            // VALIDATION
            if (requestDto.Email == null || requestDto.Password == null)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = "Invalid Student credentials",
                    Data = "Invalid Student credentials"
                });
            }

            var user = await _userManager.FindByEmailAsync(requestDto.Email);

            if (user == null)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = "Student account not existed",
                    Data = "Student account not existed"
                });
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                requestDto.Password,
                isPersistent: true,    // remember me
                lockoutOnFailure: false
            );


            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = "Invalid Student credentials",
                    Data = "Invalid Student credentials"
                });
            }
            
            
            return Json(new ApiResponse<string>
            {
                Message = "Dang nhap thanh cong",
                Data = ""
            });
        }
        

        [HttpPost("student/logout")]
        public async Task<IActionResult> HandleLogoutStudent()
        {
            await _signInManager.SignOutAsync();
            
            return Json(new ApiResponse<string>
            {
                Message = "Logout successful",
                Data = "/"
            });
        }

        [HttpPost("admin/logout")]
        public async Task<IActionResult> HandleLogoutAdmin()
        {
            await _signInManager.SignOutAsync();
            
            return Json(new ApiResponse<string>
            {
                Message = "Logout successful",
                Data = "/admin/login"
            });
        }


    }
}