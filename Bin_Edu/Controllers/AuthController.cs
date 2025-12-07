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