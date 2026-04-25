using System.Text.Json.Serialization;

namespace ProjectManagement.DTOs.Users
{
    public class AuthDTO
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public bool IsAuthenticated { get; set; }
        public DateTime ExpiresOn { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
        [JsonIgnore]
        public string? RefreshToken { get; set; }
    }
}
