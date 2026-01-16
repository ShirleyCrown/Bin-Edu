using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Controllers.RequestDto
{
    public class CreateCourseExcerciseRequest
    {
        public string ExerciseName { get; set; }
        public string ExerciseDescription { get; set; }
        public DateOnly ExerciseSubmitDeadline { get; set; }
    }
}