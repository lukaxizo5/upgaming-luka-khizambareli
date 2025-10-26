namespace Upgaming.Luka.Khizambareli.Models;

public class Book
{
    public int ID { get; set; }
    public string Title { get; set; } = String.Empty;
    public int AuthorID { get; set; }
    public int PublicationYear { get; set; }
}