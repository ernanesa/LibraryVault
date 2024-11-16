using LibraryVault.API.Contracts.Requests;
using LibraryVault.Application.Interfaces.Services;

namespace LibraryVault.API.Endpoints
{
    public static class BookEndpoints
    {
        public static void MapBookEndpoints(this WebApplication app)
        {
            var bookGroup = app.MapGroup("/books")
                .WithTags("Books");
            
            bookGroup.MapGet("/books", async (IBookService bookService, string? author, string? title, int? year) =>
            {
                var books = await bookService.SearchBooksAsync(author, title, year);
                return Results.Ok(books);
            })
            .AllowAnonymous();

            bookGroup.MapPost("/books", async (IBookService bookService, CreateBookRequest request) =>
            {
                var bookId = await bookService.AddBookAsync(request.Title, request.Author, request.Year);
                return Results.Created($"/books/{bookId}", bookId);
            })
            .RequireAuthorization();

            bookGroup.MapPut("/books/{id:int}", async (IBookService bookService, int id, UpdateBookRequest request) =>
            {
                await bookService.UpdateBookAsync(id, request.Title, request.Author, request.ISBN, request.Year);
                return Results.NoContent();
            })
            .RequireAuthorization();

            bookGroup.MapDelete("/books/{id:int}", async (IBookService bookService, int id) =>
            {
                await bookService.DeleteBookAsync(id);
                return Results.NoContent();
            })
            .RequireAuthorization();
        }
    }
}