namespace Library_API.Models
{
    public class Book
    {
        public int BookId { get; set; } 
        public string Title { get; set; }
        public string ISBN { get; set; }
        public int Quantity { get; set; }
        public int GenreId { get; set; }
    }

    public class AddBook
    {
        public string Title { get; set; }
        public string ISBN { get; set; }
        public int GenreId { get; set; }
    }

    public class UpdateBook
    {
        public string? Title { get; set; }
        public string? ISBN { get; set; }
        public int GenreId { get; set; } = 0;
    }
}
