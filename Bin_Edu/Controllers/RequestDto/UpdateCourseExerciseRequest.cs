using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Controllers.RequestDto
{
    public class UpdateCourseExerciseRequest
    {
        public string UpdateExerciseName { get; set; }
        public string UpdateExerciseDescription { get; set; }
        public DateOnly UpdateExerciseSubmitDeadline { get; set; }
    }
}