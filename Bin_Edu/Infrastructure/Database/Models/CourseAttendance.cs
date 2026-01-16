using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Infrastructure.Database.Models
{
    [Table("CourseAttendances")]
    public class CourseAttendance
    {

        public int Id { get; set; }

        public string Status { get; set; }

        public DateTime AttendedAt { get; set; }


        // Relationships
        [ForeignKey("CourseTimetable")]
        public int CourseTimetableId { get; set; }
        public CourseTimetable CourseTimetable { get; set; }


        [ForeignKey("CourseRegistration")]
        public int CourseRegistrationId { get; set; }
        public CourseRegistration CourseRegistration { get; set; }
        
    }
}