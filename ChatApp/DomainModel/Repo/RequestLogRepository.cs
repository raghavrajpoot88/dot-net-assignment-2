using ChatApp.DomainModel.Models;
using ChatApp.DomainModel.Repo.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.DomainModel.Repo
{
    public class RequestLogRepository :IRequestLog
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _applicationDbContext;

        public RequestLogRepository(UserManager<IdentityUser> userManager, ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<ICollection<RequestLog>> GetLogList(int timeInterval)
        {
            DateTime endTime = DateTime.Now;
            DateTime startTime = endTime.AddMinutes(-timeInterval); 
            
            var result = await _applicationDbContext.requestlogs
                .Where(log => (log.RequestDateTimeUtc >= startTime && log.RequestDateTimeUtc <= endTime)).OrderByDescending
                (m => m.RequestDateTimeUtc).ToListAsync();
            return result;
        }
    }
}
