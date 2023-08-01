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

        public async Task<ICollection<RequestLog>> GetLogList()
        {
            var result = await _applicationDbContext.requestlogs.ToListAsync();
            return result;
        }
    }
}
