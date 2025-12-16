using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Controllers.ResponseDto
{
    public class GetMyCoursesResponse
    {
        
        public List<MyCourses> MyCourses { get; set; }

        public int TotalPages { get; set; }

    }


    public class MyCourses
    {
        public int Id { get; set; }

        public string TeachingTeacherName { get; set; }

        public string CourseTitle { get; set; }

        public string CourseSubject { get; set; }

        public int WeekDuration { get; set; }

        public List<CourseTimetableDetail> Timetables { get; set; }
    }

}