using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Controllers.ResponseDto
{
    public class GetCoursesResponse
    {
        public int Id { get; set; }

        public string TeachingTeacherName { get; set; }

        public string CourseTitle { get; set; }

        public string CourseDescription { get; set; }

        public string CourseSubject { get; set; }

        public int WeekDuration { get; set; }

        public int NumberOfStudents { get; set; }

        public long CoursePrice { get; set; }

        public string ThumbNail { get; set; }
    }
}