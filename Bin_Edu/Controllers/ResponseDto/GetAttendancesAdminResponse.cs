using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Controllers.ResponseDto
{
    public class GetAttendancesAdminResponse
    {
        public string StudentId { get; set; }

        public string StudentFullName { get; set; }

        public string? AttendanceStatus { get; set; }

        public string? AttendedAt { get; set; }
    }
}