using System.Text.Json.Serialization;

namespace LibraryVault.API.Contracts.Responses
{
    [JsonSerializable(typeof(AuthResponse))]
    public class AuthResponse
    {
        public string Token { get; set; }

        public AuthResponse(string token)
        {
            Token = token;
        }
    }
}