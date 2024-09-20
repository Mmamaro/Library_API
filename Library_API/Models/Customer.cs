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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdNumber { get; set; }
        [EmailAddress] public string Email { get; set; }
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
