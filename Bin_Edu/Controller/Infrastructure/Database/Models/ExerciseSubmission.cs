using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Controller.Infrastructure.Database.Models
{
    public class ExerciseSubmission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public int CourseExerciseId { get; set; }

        [Required]
        public int CourseRegistrationId { get; set; }

        [Required]
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public CourseExercise CourseExercise { get; set; }
        public CourseRegistration CourseRegistration { get; set; }
    }
}