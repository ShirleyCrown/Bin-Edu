using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Controllers.RequestDto
{
    public class CreateTimetableAdminRequest
    {
        public string DayOfWeek { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }
    }
}