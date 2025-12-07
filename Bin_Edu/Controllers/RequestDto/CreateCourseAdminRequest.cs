using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Controllers.RequestDto
{
    public class CreateCourseAdminRequest
    {
        public string TeachingTeacherName { get; set; }

        public string CourseTitle { get; set; }

        public string CourseDescription { get; set; }

        public string CourseSubject { get; set; }

        public string CoursePrice { get; set; }
    }
}