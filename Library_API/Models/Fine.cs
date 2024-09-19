namespace Library_API.Models
{
    public class Fine
    {
        public int FineId { get; set; }
        public int BorrowingId { get; set; }
        public decimal Amount { get; set; }
        public string FineStatus { get; set; }
    }
}
