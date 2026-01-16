using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Controllers.ResponseDto
{
    public class GetExerciseResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateOnly SubmitDeadline { get; set; }

        public bool IsSubmitted { get; set; }

        public string SubmitFileName { get; set; }

        public float Score { get; set; }

    }
}