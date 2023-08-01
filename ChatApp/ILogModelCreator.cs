using ChatApp.DomainModel.Models;

namespace ChatApp
{
    public interface ILogModelCreator
    {
        RequestLog LogModel { get; }
        string LogString();
    }
}
