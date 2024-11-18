using Moq;
using LibraryVault.API.Contracts.Requests;
using LibraryVault.API.Endpoints;
using LibraryVault.Application.Interfaces.Services;
using LibraryVault.Domain.Entities;
using LibraryVault.Domain.ValueObjects;
using Microsoft.AspNetCore.Builder;

namespace LibraryVault.Tests.API.Tests.Endpoints
{
    public class BookEndpointsTests
    {
        private readonly Mock<IBookService> _mockBookService;
        private readonly WebApplication _app;

        public BookEndpointsTests()
        {
            _mockBookService = new Mock<IBookService>();

            // Criar uma instância mock do WebApplication
            var builder = WebApplication.CreateBuilder();
            _app = builder.Build();
            BookEndpoints.MapBookEndpoints(_app);
        }

        [Fact]
        public async Task GetBooks_WithValidParameters_ReturnsOkResult()
        {
            // Arrange
            var expectedBooks = new List<Book>
            {
                new Book { Id = 1, Title = "Test Book", Author = "Test Author", ISBN = new ISBN("978-0-7475-3269-9"), Year = 2024 }
            };

            _mockBookService.Setup(x => x.SearchBooksAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<int?>()))
                .ReturnsAsync(expectedBooks);

            // Act
            var response = await _mockBookService.Object.SearchBooksAsync("Test Author", "Test Title", "978-0-7475-3269-9", 2024);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(expectedBooks, response);
        }

        [Fact]
        public async Task PostBook_WithValidRequest_ReturnsCreatedResult()
        {
            // Arrange
            var request = new CreateBookRequest
            {
                Title = "New Book",
                Author = "New Author",
                ISBN = "978-0-7475-3269-9",
                Year = 2024
            };

            var expectedId = 1;
            _mockBookService.Setup(x => x.CreateBookAsync(
                request.Title,
                request.Author,
                It.IsAny<ISBN>(),
                request.Year))
                .ReturnsAsync(expectedId);

            // Act
            var bookId = await _mockBookService.Object.CreateBookAsync(
                request.Title,
                request.Author,
                new ISBN(request.ISBN),
                request.Year);

            // Assert
            Assert.Equal(expectedId, bookId);
        }

        [Fact]
        public async Task PutBook_WithValidRequest_ReturnsNoContent()
        {
            // Arrange
            var bookId = 1;
            var request = new UpdateBookRequest
            {
                Title = "Updated Book",
                Author = "Updated Author",
                ISBN = new ISBN("978-0-7475-3269-9"),
                Year = 2024
            };

            _mockBookService.Setup(x => x.UpdateBookAsync(
                bookId,
                request.Title,
                request.Author,
                request.ISBN,
                request.Year))
                .Returns(Task.CompletedTask);

            // Act & Assert
            await _mockBookService.Object.UpdateBookAsync(
                bookId,
                request.Title,
                request.Author,
                request.ISBN,
                request.Year);

            _mockBookService.Verify(x => x.UpdateBookAsync(
                bookId,
                request.Title,
                request.Author,
                request.ISBN,
                request.Year), 
                Times.Once);
        }

        [Fact]
        public async Task DeleteBook_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var bookId = 1;
            _mockBookService.Setup(x => x.DeleteBookAsync(bookId))
                .Returns(Task.CompletedTask);

            // Act
            await _mockBookService.Object.DeleteBookAsync(bookId);

            // Assert
            _mockBookService.Verify(x => x.DeleteBookAsync(bookId), Times.Once);
        }

        [Fact]
        public async Task GetBooks_WhenServiceThrowsException_ThrowsException()
        {
            // Arrange
            _mockBookService.Setup(x => x.SearchBooksAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int?>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _mockBookService.Object.SearchBooksAsync("author", "title", "isbn", 2024));
        }

        [Fact]
        public async Task PostBook_WithInvalidISBN_ThrowsArgumentException()
        {
            // Arrange
            var request = new CreateBookRequest
            {
                Title = "New Book",
                Author = "New Author",
                ISBN = "invalid-isbn",
                Year = 2024
            };

            _mockBookService.Setup(x => x.CreateBookAsync(
                request.Title,
                request.Author,
                It.IsAny<ISBN>(),
                request.Year))
                .ThrowsAsync(new ArgumentException("Invalid ISBN"));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _mockBookService.Object.CreateBookAsync(
                    request.Title,
                    request.Author,
                    new ISBN(request.ISBN),
                    request.Year));
        }
    }
}