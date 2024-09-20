namespace Library_API.Models
{
    public class Genre
    {
        public int GenreId { get; set; }
        public string GenreName { get; set; }
    }

    public class AddGenre
    {
        public string GenreName { get; set; }
    }
}
