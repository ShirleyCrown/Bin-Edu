using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Controller.Infrastructure.Database.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string TeachingTeacherName { get; set; }

        [Required]
        public string CourseTitle { get; set; }

        [Required]
        public string CourseDescription { get; set; }

        [Required]
        public int CourseSubject { get; set; }

        [Required]
        public long CoursePrice { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<CourseTimetable> CourseTimetables { get; set; }
        public ICollection<CourseExercise> CourseExercises { get; set; }
        public ICollection<CourseRegistration> CourseRegistrations { get; set; }
    }
}