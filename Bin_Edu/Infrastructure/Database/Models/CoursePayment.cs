using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Infrastructure.Database.Models
{
    public class CoursePayment
    {

        [Key]
        public int Id { get; set; }

        public int CourseRegistrationId { get; set; }

        public DateTime PaidAt { get; set; }

        // Navigation
        public CourseRegistration CourseRegistration { get; set; }
    }
}