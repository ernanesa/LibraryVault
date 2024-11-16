using System.Text.Json.Serialization;

namespace LibraryVault.API.Contracts.Requests
{
    [JsonSerializable(typeof(UpdateUserRequest))]
    public class UpdateUserRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
    }
}