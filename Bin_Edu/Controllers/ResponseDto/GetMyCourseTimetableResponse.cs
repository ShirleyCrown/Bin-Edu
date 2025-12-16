using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Controllers.ResponseDto
{
    public class GetMyCourseTimetableResponse
    {
        public string CourseTitle { get; set; }

        public string TeachingTeacherName { get; set; }

        public List<CourseTimetableDetail> Timetables { get; set; }

    }

}