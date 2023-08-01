using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ChatApp.MiddleLayer.DTOs
{
    public class googleLoginDTO
    {
        public const string PROVIDER = "google";

        [JsonProperty("idToken")]
        [Required]
        public string IdToken { get; set; }
    }
}
