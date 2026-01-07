using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Controllers.RequestDto
{
    public class CourseFilterRequest
    {
        public int Page { get; set; } = 0;
        public string? Keyword { get; set; }
        public string? Subject { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}