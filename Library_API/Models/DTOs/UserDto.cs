using System.ComponentModel.DataAnnotations;

namespace Library_API.Models.DTOs
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [EmailAddress] public string Email { get; set; }
        public string Role { get; set; }
        public bool Active { get; set; }
      
    }
}
