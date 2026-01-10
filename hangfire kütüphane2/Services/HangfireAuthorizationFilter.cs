using Hangfire.Dashboard;

namespace LibraryManagement.Services;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        
        // Allow all authenticated users in development
        // In production, you should restrict this to admins only
        #if DEBUG
            return true;
        #else
            return httpContext.User.Identity?.IsAuthenticated == true 
                   && httpContext.User.IsInRole("Admin");
        #endif
    }
}

