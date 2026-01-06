using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Infrastructure.Database.Models
{
    public class ExerciseSubmission
    {
        [Key]
        public int Id { get; set; }


        public string FileName { get; set; }


        public int CourseExerciseId { get; set; }


        public int CourseRegistrationId { get; set; }

        public float Score { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public CourseExercise CourseExercise { get; set; }
        public CourseRegistration CourseRegistration { get; set; }
    }
}