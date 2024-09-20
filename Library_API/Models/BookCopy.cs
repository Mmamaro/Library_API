namespace Library_API.Models
{
    public class BookCopy
    {
        public int CopyId { get; set; }
        public int BookId { get; set; }
        public string BarCode { get; set; }
        public string Status { get; set; }
    }

    public class AddBookCopy
    {
        public int BookId { get; set; }
        public string? BarCode { get; set; }
        public string? Status { get; set; }
    }
}
