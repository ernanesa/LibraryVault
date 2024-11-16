using LibraryVault.Domain.ValueObjects;

namespace LibraryVault.Domain.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public ISBN ISBN { get; set; }
        public int Year { get; set; }
    }
}