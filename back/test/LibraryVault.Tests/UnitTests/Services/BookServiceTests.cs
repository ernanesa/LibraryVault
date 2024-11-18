using Xunit;
using Moq;
using LibraryVault.Application.Interfaces.Repositories;
using LibraryVault.Domain.Entities;
using LibraryVault.Domain.ValueObjects;
using LibraryVault.Application.Services;

namespace LibraryVault.Tests.UnitTests.Services
{
    public class BookServiceTests
    {
        private readonly Mock<IBookRepository> _mockBookRepository;
        private readonly BookService _bookService;

        public BookServiceTests()
        {
            _mockBookRepository = new Mock<IBookRepository>();
            _bookService = new BookService(_mockBookRepository.Object);
        }

        [Fact]
        public async Task GetBooksAsync_ShouldReturnAllBooks()
        {
            // Arrange
            var expectedBooks = new List<Book>
            {
                new Book { Id = 1, Title = "Book1", Author = "Author1", ISBN = new ISBN("1234567890123"), Year = 2020 },
                new Book { Id = 2, Title = "Book2", Author = "Author2", ISBN = new ISBN("0987654321234"), Year = 2021 }
            };
            _mockBookRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(expectedBooks);

            // Act
            var result = await _bookService.GetBooksAsync();

            // Assert
            Assert.Equal(expectedBooks, result);
            _mockBookRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetBookByIdAsync_ShouldReturnBook_WhenBookExists()
        {
            // Arrange
            var expectedBook = new Book { Id = 1, Title = "Book1", Author = "Author1", ISBN = new ISBN("1234567890123"), Year = 2020 };
            _mockBookRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(expectedBook);

            // Act
            var result = await _bookService.GetBookByIdAsync(1);

            // Assert
            Assert.Equal(expectedBook, result);
            _mockBookRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task CreateBookAsync_ShouldReturnId_WhenBookCreated()
        {
            // Arrange
            var title = "New Book";
            var author = "New Author";
            var isbn = new ISBN("1234567890123");
            var year = 2023;

            Book savedBook = null;
            _mockBookRepository.Setup(repo => repo.AddAsync(It.IsAny<Book>()))
                .Callback<Book>(book => savedBook = book)
                .Returns(Task.CompletedTask);

            // Act
            await _bookService.CreateBookAsync(title, author, isbn, year);

            // Assert
            Assert.NotNull(savedBook);
            Assert.Equal(title, savedBook.Title);
            Assert.Equal(author, savedBook.Author);
            Assert.Equal(isbn, savedBook.ISBN);
            Assert.Equal(year, savedBook.Year);
            _mockBookRepository.Verify(repo => repo.AddAsync(It.IsAny<Book>()), Times.Once);
        }

        [Fact]
        public async Task UpdateBookAsync_ShouldUpdateBook_WhenBookExists()
        {
            // Arrange
            var existingBook = new Book { Id = 1, Title = "Old Title", Author = "Old Author", ISBN = new ISBN("0000000000000"), Year = 2020 };
            var newTitle = "Updated Title";
            var newAuthor = "Updated Author";
            var newISBN = new ISBN("1234567890123");
            var newYear = 2023;

            _mockBookRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingBook);
            _mockBookRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);

            // Act
            await _bookService.UpdateBookAsync(1, newTitle, newAuthor, newISBN, newYear);

            // Assert
            Assert.Equal(newTitle, existingBook.Title);
            Assert.Equal(newAuthor, existingBook.Author);
            Assert.Equal(newISBN, existingBook.ISBN);
            Assert.Equal(newYear, existingBook.Year);
            _mockBookRepository.Verify(repo => repo.UpdateAsync(existingBook), Times.Once);
        }

        [Fact]
        public async Task UpdateBookAsync_ShouldThrowException_WhenBookNotFound()
        {
            // Arrange
            _mockBookRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Book)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _bookService.UpdateBookAsync(1, "Title", "Author", new ISBN("1234567890123"), 2023));
        }

        [Fact]
        public async Task DeleteBookAsync_ShouldCallRepository()
        {
            // Arrange
            _mockBookRepository.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            await _bookService.DeleteBookAsync(1);

            // Assert
            _mockBookRepository.Verify(repo => repo.DeleteAsync(1), Times.Once);
        }

        [Theory]
        [InlineData("Author1", null, null, null)]
        [InlineData(null, "Title1", null, null)]
        [InlineData(null, null, "1234567890123", null)]
        [InlineData(null, null, null, 2020)]
        public async Task SearchBooksAsync_ShouldFilterBooks_BasedOnCriteria(string author, string title, string isbn, int? year)
        {
            // Arrange
            var books = new List<Book>
            {
                new Book { Id = 1, Title = "Title1", Author = "Author1", ISBN = new ISBN("1234567890123"), Year = 2020 },
                new Book { Id = 2, Title = "Title2", Author = "Author2", ISBN = new ISBN("0987654321234"), Year = 2021 }
            };
            _mockBookRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(books);

            // Act
            var result = await _bookService.SearchBooksAsync(author, title, isbn, year);

            // Assert
            Assert.NotNull(result);
            if (author != null)
                Assert.All(result, book => Assert.Contains(author, book.Author, StringComparison.OrdinalIgnoreCase));
            if (title != null)
                Assert.All(result, book => Assert.Contains(title, book.Title, StringComparison.OrdinalIgnoreCase));
            if (isbn != null)
                Assert.All(result, book => Assert.Contains(isbn, book.ISBN.Value, StringComparison.OrdinalIgnoreCase));
            if (year.HasValue)
                Assert.All(result, book => Assert.Equal(year.Value, book.Year));
        }
    }
}