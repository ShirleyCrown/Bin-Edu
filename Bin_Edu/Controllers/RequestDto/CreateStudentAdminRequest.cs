using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bin_Edu.Controllers.RequestDto
{
    public class CreateStudentAdminRequest
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public DateOnly Dob { get; set; }

        public string PhoneNumber { get; set; }

        public string Grade { get; set; }

        public string School { get; set; }

    }
}