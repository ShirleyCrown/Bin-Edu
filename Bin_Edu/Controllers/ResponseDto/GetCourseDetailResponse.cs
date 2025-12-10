using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Controllers.ResponseDto
{
    public class GetCourseDetailResponse
    {

        public CourseDetail CourseDetail { get; set; }

        public List<RelatedCourse> RelatedCourses { get; set; }
        
    }


    public class CourseDetail
    {
        public int Id { get; set; }

        public string TeachingTeacherName { get; set; }

        public string CourseTitle { get; set; }

        public string CourseDescription { get; set; }

        public string CourseSubject { get; set; }

        public int NumberOfStudents { get; set; }

        public long CoursePrice { get; set; }

        public int WeekDuration { get; set; }

        public List<CourseTimetableDetail> Timetables { get; set; }
    }

    public class RelatedCourse
    {
        public int Id { get; set; }

        public string TeachingTeacherName { get; set; }

        public string CourseTitle { get; set; }

        public string CourseDescription { get; set; }

        public string CourseSubject { get; set; }

        public int WeekDuration { get; set; }

        public int NumberOfStudents { get; set; }

        public long CoursePrice { get; set; }
    }

    public class CourseTimetableDetail
    {
        public string DayOfWeek { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }
    }

}