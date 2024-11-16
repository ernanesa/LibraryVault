using System.Text.Json.Serialization;

namespace LibraryVault.API.Contracts.Requests
{
    [JsonSerializable(typeof(CreateUserRequest))]
    public class CreateUserRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
    }
}