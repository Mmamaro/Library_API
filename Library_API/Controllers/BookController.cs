using Library_API.Models;
using Library_API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library_API.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBook _repo;
        private readonly ILogger<BookController> _logger;
        private readonly IGenre _genreRepo;

        public BookController(IBook repo, ILogger<BookController> logger, IGenre genreRepo)
        {
            _repo = repo;
            _logger = logger;
            _genreRepo = genreRepo;
        }

        [HttpPost]
        public ActionResult AddBook(AddBook request)
        {
            try
            {

                var book = _repo.GetBookByIsbn(request.ISBN);

                if(book != null)
                {
                    return BadRequest(new { Message = "Duplicate ISBN" });
                }

                var genre = _genreRepo.GetGenreById(request.GenreId);

                if(genre == null)
                {
                    return BadRequest(new { Message = "Genre does not exist" });
                }

                var isAdded = _repo.AddBook(request);

                if (!isAdded)
                {
                    return BadRequest(new { Message = "Could not add book" });
                }

                return Ok(new { Message = "Book added successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Book Controller while trying to add a book: {ex}",ex.Message);
                return BadRequest();
            }
        }

        [HttpGet]
        public ActionResult GetBooks()
        {
            try
            {
                var books = _repo.GetBooks();

                if(books == null)
                {
                    return NotFound(new { Message = "Could not find any books" });
                }

                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Book Controller while trying to get all books: {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("get-bookbyid/{id}")]
        public ActionResult GetBookById(int id)
        {
            try
            {
                if(id <= 0)
                {
                    return BadRequest(new { Message = "Provide valid id" });
                }

                var book = _repo.GetBookById(id);

                if (book == null)
                {
                    return NotFound();
                }

                return Ok(book);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Book Controller while trying to get a book by id: {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("get-booksbygenreid/{id}")]
        public ActionResult GetBooksByGenreId(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "Provide valid id" });
                }

                var books = _repo.GetBooksByGenreId(id);

                if (books == null)
                {
                    return NotFound();
                }

                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Book Controller while trying to get books by genre id: {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public ActionResult UpdateBook(int id, AddBook request)
        {
            try
            {
                if (id <= 0 )
                {
                    return BadRequest(new { Message = "Provide valid id" });
                }

                var book = _repo.GetBookById(id);

                if (book == null)
                {
                    return NotFound();
                }

                var bookExists = _repo.GetBookByIsbn(request.ISBN);

                if (bookExists != null)
                {
                    return BadRequest(new { Message = "Duplicate ISBN" });
                }

                var isUpdated = _repo.UpdateBook(id, request);

                if (!isUpdated)
                {
                    return BadRequest(new { Message = "Could not update book" });
                }

                return Ok(new { Message = "Book updated successfully" });

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Book Controller while trying update a book : {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteBook(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "Provide valid id" });
                }

                var book = _repo.GetBookById(id);

                if (book == null)
                {
                    return NotFound();
                }

                var isDeleted = _repo.DeleteBook(id);

                if (!isDeleted)
                {
                    return BadRequest(new { Message = "Could not delete Book" });
                }

                return Ok(new { Message = "Book deleted successfully" });

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Book Controller while trying to delete a book: {ex}", ex.Message);
                return BadRequest();
            }
        }
    }
}
