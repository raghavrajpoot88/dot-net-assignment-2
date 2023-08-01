using System.ComponentModel.DataAnnotations;

namespace ChatApp.DomainModel.Models
{
    public class RequestLog
    {
        [Key]
        public string LogId { get; set; }
        public string ClientIp { get; set; }
        public string TraceId { get; set; }
        
        //request body
        public string RequestBody { get; set; }
        public DateTime? RequestDateTimeUtc { get; set; }
        public string? Username { get; set; }

        public RequestLog()
        {
            LogId = Guid.NewGuid().ToString();
        }
    }
}
