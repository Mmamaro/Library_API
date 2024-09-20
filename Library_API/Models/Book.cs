namespace Library_API.Models
{
    public class Book
    {
        public int BookId { get; set; } 
        public string Title { get; set; }
        public string ISBN { get; set; }
        public string GenreName { get; set; }
        public int Quantity { get; set; }
    }
}
