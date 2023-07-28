
using System.ComponentModel.DataAnnotations;

namespace ChatApp.DomainModel.Models
{
    public class Messages
    {
        [Key]
        public Guid MsgId { get; set; }
        public string Id { get; set; } //Foreign Key
        public string ReceiverId { get; set; } //Foreign Key
        public string MsgBody { get; set; }
        public DateTime TimeStamp { get; set; }
       

        
    }
}
