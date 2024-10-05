using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "Missing BookId")] public int BookId { get; set; }
        [Required(ErrorMessage = "Missing BarCode")] public string BarCode { get; set; }
        [Required(ErrorMessage = "Missing Status")] public string Status { get; set; }
    }
}
