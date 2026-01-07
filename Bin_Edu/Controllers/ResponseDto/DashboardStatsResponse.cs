using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Controllers.ResponseDto
{
    public class DashboardStatsResponse
    {
        public int TotalCourses { get; set; }
        public int TotalStudents { get; set; }
        public List<MonthlyRevenueDto> MonthlyRevenue { get; set; }
    }

    public class MonthlyRevenueDto
    {
        public int Month { get; set; }   // 1 - 12
        public long Revenue { get; set; }
    }
}