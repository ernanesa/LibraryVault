using LibraryVault.Domain.Entities;
using LibraryVault.Domain.ValueObjects;

namespace LibraryVault.Application.Interfaces.Services
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetBooksAsync();
        Task<int> CreateBookAsync(string title, string author, ISBN isbn, int year);
        Task UpdateBookAsync(int id, string title, string author, ISBN isbn, int year);
        Task DeleteBookAsync(int id);
        Task<IEnumerable<Book>> SearchBooksAsync(string? author, string? title, int? year);
        Task<Book> AddBookAsync(string requestTitle, string requestAuthor, int requestYear);
    }
}