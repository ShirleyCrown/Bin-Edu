using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Controller.Infrastructure.Database.Models
{
    public class CourseExercise
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public string ExerciseName { get; set; }

        [Required]
        public string ExerciseDescription { get; set; }

        [Required]
        public DateTime ExerciseSubmitDeadline { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Course Course { get; set; }
        public ICollection<ExerciseSubmission> ExerciseSubmissions { get; set; }
    }
}