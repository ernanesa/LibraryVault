using System.Text.Json.Serialization;
using LibraryVault.Domain.ValueObjects;

namespace LibraryVault.API.Contracts.Requests
{
    public class UpdateBookRequest
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public ISBN ISBN { get; set; }
        public int Year { get; set; }
    }
}