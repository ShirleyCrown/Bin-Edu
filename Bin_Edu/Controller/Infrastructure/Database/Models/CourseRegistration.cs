using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Controller.Infrastructure.Database.Models
{
    public class CourseRegistration
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public AppUser Student { get; set; }
        public Course Course { get; set; }
        public ICollection<ExerciseSubmission> ExerciseSubmissions { get; set; }
        public ICollection<CoursePayment> CoursePayments { get; set; }
    }
}