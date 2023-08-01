using System.ComponentModel.DataAnnotations;

namespace ChatApp.MiddleLayer.DTOs
{
    public class loginDTO
    {
        public string Email { get; set; }
        
        public string? Password { get; set; }
    }
}
