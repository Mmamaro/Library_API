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
        public bool Active { get; set; }
    }

    public class AddUser
    {


        [Required(ErrorMessage = "Missing FirstName")] public string FirstName { get; set; }
        [Required(ErrorMessage = "Missing LastName")] public string LastName { get; set; }
        [Required(ErrorMessage = "Missing Email")] [EmailAddress]public string Email { get; set; }
        [Required(ErrorMessage = "Missing Password")] public string Password { get; set; }
    }

    public class UpdateUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "Missing Email")] [EmailAddress] public string Email { get; set; }
        [Required(ErrorMessage = "Missing Password")] public string Password { get; set; }
    }

}
