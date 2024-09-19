namespace Library_API.Models
{
    public class BookCopy
    {
        public int CopyId { get; set; }
        public int BookId { get; set; }
        public int BarCode { get; set; }
        public int Status { get; set; }
    }
}
