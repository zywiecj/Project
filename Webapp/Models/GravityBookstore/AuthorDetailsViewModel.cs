namespace Webapp.Models.GravityBookstore;

public class AuthorDetailsViewModel
{
    public int AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public IEnumerable<BookBasicInfo> Books { get; set; }
}

public class BookBasicInfo
{
    public int BookId { get; set; }
    public string? Title { get; set; }
}