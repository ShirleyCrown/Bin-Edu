using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Bin_Edu.Infrastructure;
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


    }
}