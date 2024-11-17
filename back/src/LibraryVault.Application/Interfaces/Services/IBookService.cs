using LibraryVault.Domain.Entities;
using LibraryVault.Domain.ValueObjects;

namespace LibraryVault.Application.Interfaces.Services
{
    public interface IBookService
    {
        Task<int> CreateBookAsync(string title, string author, ISBN isbn, int year);
        Task UpdateBookAsync(int id, string title, string author, ISBN isbn, int year);
        Task DeleteBookAsync(int id);
        Task<IEnumerable<Book>> SearchBooksAsync(string? author, string? title, string? isbn, int? year);
        Task<Book> GetBookByIdAsync(int bookId);
    }
}