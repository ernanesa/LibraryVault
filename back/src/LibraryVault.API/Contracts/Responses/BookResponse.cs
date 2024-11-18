using System.Text.Json.Serialization;

namespace LibraryVault.API.Contracts.Responses
{
    [JsonSerializable(typeof(BookResponse))]
    public class BookResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int Year { get; set; }
    }
}