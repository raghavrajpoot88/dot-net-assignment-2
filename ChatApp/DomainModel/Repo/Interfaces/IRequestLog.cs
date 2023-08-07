using ChatApp.DomainModel.Models;

namespace ChatApp.DomainModel.Repo.Interfaces
{
    public interface IRequestLog
    {
        Task<ICollection<RequestLog>> GetLogList(int timeInterval);
    }
}
