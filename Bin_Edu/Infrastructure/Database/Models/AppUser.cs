using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Bin_Edu.Infrastructure.Database.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }

        public DateOnly Dob { get; set; }

        public string Grade { get; set; }

        public string School { get; set; }
    }
}