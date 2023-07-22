namespace ChatApp.MiddleLayer.DTOs
{
    public class MessagesDTO
    {
        public Guid ReceiverId { get; set; } 
        public string MsgBody { get; set; }
    }
}
