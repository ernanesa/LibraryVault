using LibraryVault.Application.Interfaces.Repositories;
using LibraryVault.Application.Interfaces.Services;
using LibraryVault.Domain.Entities;
using LibraryVault.Domain.ValueObjects;

namespace LibraryVault.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<Book>> GetBooksAsync()
        {
            return await _bookRepository.GetAllAsync();
        }
        
        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _bookRepository.GetByIdAsync(id);
        }

        public async Task<int> CreateBookAsync(string title, string author, ISBN isbn, int year)
        {
            var book = new Book
            {
                Title = title,
                Author = author,
                ISBN = isbn,
                Year = year
            };

            await _bookRepository.AddAsync(book);
            return book.Id;
        }

        public async Task UpdateBookAsync(int id, string title, string author, ISBN isbn, int year)
        {
            var book = await _bookRepository.GetByIdAsync(id) ?? throw new Exception("Book not found.");

            book.Title = title;
            book.Author = author;
            book.ISBN = isbn;
            book.Year = year;

            await _bookRepository.UpdateAsync(book);
        }

        public async Task DeleteBookAsync(int id)
        {
            await _bookRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Book>> SearchBooksAsync(string? author, string? title, string? isbn, int? year)
        {
            var books = await _bookRepository.GetAllAsync();

            if (!string.IsNullOrEmpty(author)) { books = books.Where(b => b.Author.Contains(author, StringComparison.OrdinalIgnoreCase)); }
            if (!string.IsNullOrEmpty(title)) { books = books.Where(b => b.Title.Contains(title, StringComparison.OrdinalIgnoreCase)); }
            if (!string.IsNullOrEmpty(isbn)) { books = books.Where(b => b.ISBN.Value.Contains(isbn, StringComparison.OrdinalIgnoreCase)); }
            if (year.HasValue) { books = books.Where(b => b.Year == year.Value); }

            return books;
        }
    }
}