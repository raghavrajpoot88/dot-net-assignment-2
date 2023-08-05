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

        public async Task<ICollection<RequestLog>> GetLogList(string timeInterval)
        {
            DateTime startTime;
            DateTime endTime = DateTime.Now;
            switch (timeInterval)
            {
                case "5min":
                    startTime = endTime.AddMinutes(-5);
                    break;
                case "10min":
                    startTime = endTime.AddMinutes(-10);
                    break;
                case "30min":
                    startTime = endTime.AddMinutes(-30);
                    break;
                default:
                    startTime = DateTime.MinValue;
                    break;
            }
            var result = await _applicationDbContext.requestlogs.Where(log =>( log.RequestDateTimeUtc >= startTime && log.RequestDateTimeUtc <= endTime)).ToListAsync();
            return result;
        }
    }
}
