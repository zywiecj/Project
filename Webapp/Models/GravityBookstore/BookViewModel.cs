namespace Webapp.Models.GravityBookstore;

public class BookViewModel
{
    public int BookId { get; set; }
    public string Title { get; set; }
    public string Isbn13 { get; set; }
    public int NumPages { get; set; }
    public DateOnly? PublicationDate { get; set; }
    public int AuthorCount { get; set; }
    public int SoldCopies { get; set; }
}