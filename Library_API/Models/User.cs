using System.ComponentModel.DataAnnotations;

namespace Library_API.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [EmailAddress] public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public bool isFirstSignIn { get; set; }
        public bool isTwoFaVerified { get; set; }
        public string? twoFaKey { get; set; }
        public string? QRCode { get; set; }
        public string? ManualCode { get; set; }
    }
}
