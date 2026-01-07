using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Infrastructure.Database.Models
{
    [Table("Subjects")]
    public class Subject
    {
        public int Id { get; set; }

        public string SubjectName { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        

        // Relationships 
        public ICollection<Course> Courses { get; set; }
    }
}