using System.ComponentModel.DataAnnotations;

namespace Library_API.Models
{
    public class Borrowing
    {
        public int BorrowingId { get; set; }
        public int CustomerId { get; set; }
        public int CopyId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Status { get; set; }
    }

    public class AddBorrowing
    {
        [Required(ErrorMessage = "Missing CustomerId")] public int CustomerId { get; set; }
        [Required(ErrorMessage = "Missing CopyId")] public int CopyId { get; set; }
        [Required(ErrorMessage = "Missing DueDate")] public DateTime DueDate { get; set; }
    }

    public class UpdateBorrowing
    {
        public DateTime? BorrowDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }

    public class ReturnBorrowing
    {
        public DateTime ReturnDate { get; set; }
        public string Status { get; set; }
    }

    public class DetailedBorrowing
    {
        public int BorrowingId { get; set; }
        public int CustomerId { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public int CopyId { get; set; }
        public string BarCode { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Status { get; set; }
    }

    public class FilterBorrowing
    {
        public int? BorrowingId { get; set; }
        public int? CustomerId { get; set; }
        public int? CopyId { get; set; }
        public DateTime? endDate { get; set; }
        public DateTime? startDate { get; set; }
        public string? Status { get; set; }
    }
}
