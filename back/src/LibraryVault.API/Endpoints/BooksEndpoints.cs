using LibraryVault.API.Contracts.Requests;
using LibraryVault.Application.Interfaces.Services;
using LibraryVault.Domain.ValueObjects;

namespace LibraryVault.API.Endpoints
{
    public static class BookEndpoints
    {
        public static void MapBookEndpoints(this WebApplication app)
        {
            var bookGroup = app.MapGroup("/books")
                .WithTags("Books");

            bookGroup.MapGet("/books", async (IBookService bookService, string? author, string? isbn, string? title, int? year) =>
            {
                var books = await bookService.SearchBooksAsync(author, title, isbn, year);
                return Results.Ok(books);
            })
            .AllowAnonymous();

            bookGroup.MapPost("/books", async (IBookService bookService, CreateBookRequest request) =>
            {
                var bookId = await bookService.CreateBookAsync(request.Title, request.Author,
                    new ISBN(request.ISBN), request.Year);
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