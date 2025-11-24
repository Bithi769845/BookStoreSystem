using Newtonsoft.Json;

namespace BookStore.DTOs
{
    public class LoginDto
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
