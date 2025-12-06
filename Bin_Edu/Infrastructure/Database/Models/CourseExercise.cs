using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Infrastructure.Database.Models
{
    public class CourseExercise
    {
        [Key]
        public int Id { get; set; }

        public int CourseId { get; set; }

        public string ExerciseName { get; set; }

        public string ExerciseDescription { get; set; }

        public DateTime ExerciseSubmitDeadline { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation
        public Course Course { get; set; }
        public ICollection<ExerciseSubmission> ExerciseSubmissions { get; set; }
    }
}