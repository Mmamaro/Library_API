using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "Missing Title")] public string Title { get; set; }
        [Required(ErrorMessage = "Missing ISBN")] public string ISBN { get; set; }
        [Required(ErrorMessage = "Missing GenreId")] public int GenreId { get; set; }
    }

    public class UpdateBook
    {
        public string? Title { get; set; }
        public string? ISBN { get; set; }
        [Required(ErrorMessage = "Missing GenreId")] public int GenreId { get; set; } = 0;
    }
}
