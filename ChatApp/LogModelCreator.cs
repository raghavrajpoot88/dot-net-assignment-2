using ChatApp.DomainModel.Models;
using Newtonsoft.Json;

namespace ChatApp
{
    public class LogModelCreator : ILogModelCreator
    {
        public RequestLog LogModel { get; private set; }

        public LogModelCreator()
        {
            LogModel = new RequestLog();
        }

        public string LogString()
        {
            var logResult = JsonConvert.SerializeObject(LogModel);
            return logResult;
        }
    }
}
