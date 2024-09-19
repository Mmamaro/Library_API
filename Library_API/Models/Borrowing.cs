namespace Library_API.Models
{
    public class Borrowing
    {
        public int BorrowingId { get; set; }
        public int UserId { get; set; }
        public int CopyId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Status { get; set; }
    }
}
