using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Upgaming.Luka.Khizambareli.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

List<Author> _authors = new List<Author>
{
    new Author { ID = 1, Name = "Robert C. Martin" },
    new Author { ID = 2, Name = "Jeffrey Richter" }
};

List<Book> _books = new List<Book>
{
    new Book { ID = 1, Title = "Clean Code", AuthorID = 1, PublicationYear = 2008 },
    new Book { ID = 2, Title = "CLR via C#", AuthorID = 2, PublicationYear = 2012 },
    new Book { ID = 3, Title = "The Clean Coder", AuthorID = 1, PublicationYear = 2011 }
};

app.MapGet("/api/books", () =>
{
    var bookDtos = _books.Select(book =>
    {
        var author = _authors.FirstOrDefault(a => a.ID == book.AuthorID);
        return new BookDto
        {
            ID = book.ID,
            Title = book.Title,
            AuthorName = author != null ? author.Name : "Unknown",
            PublicationYear = book.PublicationYear
        };
    }).ToList();

    return Results.Ok(bookDtos);
});

app.MapGet("/api/authors/{id}/books", (int id) =>
{
    var author = _authors.FirstOrDefault(a => a.ID == id);
    if (author == null)
        return Results.NotFound($"Author with ID {id} not found.");

    var booksByAuthor = _books
        .Where(b => b.AuthorID == id)
        .Select(b => new BookDto
        {
            ID = b.ID,
            Title = b.Title,
            AuthorName = author.Name,
            PublicationYear = b.PublicationYear
        })
        .ToList();

    return Results.Ok(booksByAuthor);
});

app.MapPost("/api/books", (Book newBook) =>
{
    // validation logic
    if (string.IsNullOrWhiteSpace(newBook.Title))
        return Results.BadRequest("Title cannot be empty.");

    if (!_authors.Any(a => a.ID == newBook.AuthorID))
        return Results.BadRequest("AuthorID does not exist.");

    if (newBook.PublicationYear > DateTime.Now.Year)
        return Results.BadRequest("PublicationYear cannot be in the future.");

    // generate new id with auto increment to match sql
    newBook.ID = _books.Max(b => b.ID) + 1;

    _books.Add(newBook);

    var author = _authors.First(a => a.ID == newBook.AuthorID);

    return Results.Created($"/api/books/{newBook.ID}", newBook);
});

app.Run();