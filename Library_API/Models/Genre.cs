using System.ComponentModel.DataAnnotations;

namespace Library_API.Models
{
    public class Genre
    {
        public int GenreId { get; set; }
        public string GenreName { get; set; }
    }

    public class AddGenre
    {
        [Required(ErrorMessage = "Missing GenreName")] public string GenreName { get; set; }
    }
}
