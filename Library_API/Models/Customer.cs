using System.ComponentModel.DataAnnotations;

namespace Library_API.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdNumber { get; set; }
        [EmailAddress] public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class AddCustomer
    {
        [Required(ErrorMessage = "Missing FirstName")] 
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Missing LastName")] 
        public string LastName { get; set; }

        [ Required(ErrorMessage = "Missing Id Number")]
        [StringLength( maximumLength: 13, MinimumLength = 13,ErrorMessage = "Id is must be 13 characters")] 
        public string IdNumber { get; set; }

        [Required(ErrorMessage = "Missing Email")]
        [EmailAddress] 
        public string Email { get; set; }

        [Required(ErrorMessage = "Missing Phone Number")] 
        public string Phone { get; set; }
    }

    public class UpdateCustomer
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? IdNumber { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
