using ChatApp.DomainModel.Models;

namespace ChatApp.MiddleLayer.Services
{
    public interface IRequestLogsService
    {
        Task<ICollection<RequestLog>> GetLogs(int timeInterval);
    }
}
