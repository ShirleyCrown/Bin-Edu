using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Infrastructure.Database.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        public string TeachingTeacherName { get; set; }

        public string CourseTitle { get; set; }

        public string CourseDescription { get; set; }

        public long CoursePrice { get; set; }

        public DateOnly OpeningDate { get; set; }

        public DateOnly EndDate { get; set; }
        public byte[]? ThumbNail { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;



        // Navigation
        [ForeignKey("Subject")]
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }

        public ICollection<CourseTimetable> CourseTimetables { get; set; }
        public ICollection<CourseExercise> CourseExercises { get; set; }
        public ICollection<CourseRegistration> CourseRegistrations { get; set; }
    }
}