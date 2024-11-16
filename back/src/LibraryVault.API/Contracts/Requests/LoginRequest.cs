using System.Text.Json.Serialization;

namespace LibraryVault.API.Contracts.Requests
{
    [JsonSerializable(typeof(LoginRequest))]
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}