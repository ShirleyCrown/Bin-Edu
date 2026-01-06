using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Controllers.ResponseDto
{
    public class GetSubmissionResponse
    {
        public int Id { get; set; }
        public string StudentName { get; set; }
        public DateOnly SubmittedAt { get; set; }
    }
}