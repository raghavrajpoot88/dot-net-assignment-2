using System.ComponentModel.DataAnnotations;

namespace ChatApp.MiddleLayer.DTOs
{
    public class loginDTO
    {
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
