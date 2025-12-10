using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Infrastructure.Database.Models
{
    public class CourseTimetable
    {
        [Key]
        public int Id { get; set; }

        public int CourseId { get; set; }

        public string DayOfWeek { get; set; }

        public DateOnly StartDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Course Course { get; set; }
    }
}