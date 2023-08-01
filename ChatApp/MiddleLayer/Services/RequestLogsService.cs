using ChatApp.DomainModel.Models;
using ChatApp.DomainModel.Repo.Interfaces;
using System.Reflection.Metadata.Ecma335;

namespace ChatApp.MiddleLayer.Services
{
    public class RequestLogsService : IRequestLogsService
    {
        private readonly IRequestLog _requestLog;

        public RequestLogsService(IRequestLog requestLogs)
        {
            _requestLog = requestLogs;
        }
        public async Task<ICollection<RequestLog>> GetLogs()
        {
           return await _requestLog.GetLogList();
        }

    }
}
