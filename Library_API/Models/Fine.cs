namespace Library_API.Models
{
    public class Fine
    {
        public int FineId { get; set; }
        public int BorrowingId { get; set; }
        public decimal Amount { get; set; }
        public string FineStatus { get; set; }
    }

    public class AddFine
    {
        public int BorrowingId { get; set; }
        public decimal Amount { get; set; }
        public string FineStatus { get; set; }
    }

    public class UpdateFine
    {
        public int? BorrowingId { get; set; }
        public decimal? Amount { get; set; }
        public string? FineStatus { get; set; }
    }

    public class DetailedFine
    {
        public int FineId { get; set; }
        public int BorrowingId { get; set; }
        public int CustomerId { get; set; }
        public string Email { get; set; }
        public decimal Amount { get; set; }
        public string FineStatus { get; set; }
    }
}
