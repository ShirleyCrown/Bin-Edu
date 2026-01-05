using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Controllers.ResponseDto
{
    public class GetMyCourseDetailResponse
    {
        public int Id { get; set; }

        public string TeachingTeacherName { get; set; }

        public string CourseTitle { get; set; }

        public string CourseDescription { get; set; }

        public string CourseSubject { get; set; }

        public int WeekDuration { get; set; }

        public List<CourseTimetableDetail> Timetables { get; set; }
    }
}