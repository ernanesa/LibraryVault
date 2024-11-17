using System.Text.Json.Serialization;

namespace LibraryVault.API.Contracts.Requests
{
    public class CreateBookRequest
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int Year { get; set; }
    }
}