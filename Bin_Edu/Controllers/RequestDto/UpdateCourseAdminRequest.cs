using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Controllers.RequestDto
{
    public class UpdateCourseAdminRequest
    {
        public string UpdateTeachingTeacherName { get; set; }

        public string UpdateCourseTitle { get; set; }

        public string UpdateCourseDescription { get; set; }

        public string UpdateCourseSubject { get; set; }

        public string UpdateCoursePrice { get; set; }

        public string UpdateOpeningDate { get; set; }

        public string UpdateEndDate { get; set; }
    }
}