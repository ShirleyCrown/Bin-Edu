using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire.Dashboard;

namespace Student_Science_Research_Management_UEF.Infrastructure.Mail
{
    public class HangfireCustomAuthorizeFilter : Hangfire.Dashboard.IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            return httpContext.User.Identity?.IsAuthenticated == true;
        }
    }
}