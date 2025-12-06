using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Controller.Infrastructure.Database.Models
{
    public class CoursePayment
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public int CourseRegistrationId { get; set; }

        [Required]
        public DateTime PaidAt { get; set; }

        // Navigation
        public CourseRegistration CourseRegistration { get; set; }
    }
}