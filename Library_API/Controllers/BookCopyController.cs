using Azure.Core;
using Library_API.Models;
using Library_API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library_API.Controllers
{
    [Route("api/bookcopies")]
    [ApiController]
    public class BookCopyController : ControllerBase
    {
        private readonly IBookCopy _repo;
        private readonly IBook _bookRepo;
        private readonly ILogger<BookCopyController> _logger;

        public BookCopyController(IBookCopy repo, ILogger<BookCopyController> logger, IBook bookRepo)
        {
            _logger = logger;
            _repo = repo;
            _bookRepo = bookRepo;
        }

        [HttpPost]
        public ActionResult AddBookCopy(AddBookCopy request)
        {
            try
            {
                var validStatuses = new List<string> { "available", "lost", "borrowed" };

                if (string.IsNullOrWhiteSpace(request.BarCode) || string.IsNullOrWhiteSpace(request.Status) || request.BookId <= 0)
                {
                    return BadRequest(new { Message = "Provide all necessary fields" });
                }

                if (!validStatuses.Contains(request.Status.ToLower()))
                {
                    return BadRequest(new { Message = "There is no such status" });
                }


                var book = _bookRepo.GetBookById(request.BookId);

                if (book == null)
                {
                    return NotFound(new { Message = "Book does not exist" });
                }

                var isAdded = _repo.AddBookCopy(request);

                if(!isAdded)
                {
                    return BadRequest(new { Message = "Could not add book copy" });
                }

                return Ok(new { Message = "Book copy added successfully" });

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Book copy Controller while trying to add a book copy: {ex}",ex.Message);
                return BadRequest();
            }
        }

        [HttpGet]
        public ActionResult GetBookCopies()
        {
            try
            {
               
                var bookCopies = _repo.GetBookCopies();

                if(bookCopies == null)
                {
                    return NotFound(new { Message = "Could not get any books" });
                }

                return Ok(bookCopies);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Book copy Controller while trying to get all book copies: {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("get-copybyid/{id}")]
        public ActionResult GetBookCopyById(int id)
        {
            try
            {
                if ( id <= 0)
                {
                    return BadRequest(new { Message = "Provide valid id" });
                }

                var bookCopy = _repo.GetBookCopyById(id);

                if (bookCopy == null)
                {
                    return NotFound();
                }

                return Ok(bookCopy);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Book copy Controller while trying to get book copy by id: {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("get-copiesbybookid/{id}")]
        public ActionResult GetBookCopiesByBookId(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "Provide valid id" });
                }

                var bookCopies = _repo.GetBookCopiesByBookId(id);

                if (bookCopies == null)
                {
                    return NotFound();
                }

                return Ok(bookCopies);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Book copy Controller while trying to add a book copy: {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("get-copiesbystatus/{status}")]
        public ActionResult GetBookCopiesByStatus(string status)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(status))
                {
                    return BadRequest(new { Message = "Provide status" });
                }

                var bookCopies = _repo.GetBookCopiesByStatus(status);

                if (bookCopies == null)
                {
                    return NotFound();
                }

                return Ok(bookCopies);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Book copy Controller while trying to get a book copy by status: {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public ActionResult UpdateBook(int id, AddBookCopy request)
        {
            try
            {
                var validStatuses = new List<string> { "available", "lost", "borrowed" };


                if ( id <= 0)
                {
                    return BadRequest(new { Message = "Provide valid id" });
                }

                if (!string.IsNullOrWhiteSpace(request.Status))
                {
                    if (!validStatuses.Contains(request.Status.ToLower()))
                    {
                        return BadRequest(new { Message = "There is no such status" });
                    }
                }

                var book = _bookRepo.GetBookById(request.BookId);

                if (book == null)
                {
                    return NotFound(new { Message = "Book does not exist" });
                }

                var bookCopy = _repo.GetBookCopyById(id);

                if (bookCopy == null)
                {
                    return NotFound(new { Message = "Book copy does not exist" });
                }

                if(!string.IsNullOrWhiteSpace(request.BarCode))
                {
                    var isBarCodeExist = _repo.GetBookCopyByBarCode(request.BarCode);

                    if (isBarCodeExist != null)
                    {
                        return BadRequest(new { Message = "BarCode already exists" });
                    }
                }

                var isUpdated = _repo.UpdateBookCopy(id, request);

                if (!isUpdated)
                {
                    return BadRequest(new { Message = "Could not update book copy" });
                }

                return Ok(new { Message = "Book copy updated successfully" });

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Book copy Controller while trying to update a book copy: {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteBook(int id)
        {
            try
            {
                if ( id <= 0)
                {
                    return BadRequest(new { Message = "Provide valid id" });
                }

                var bookCopy = _repo.GetBookCopyById(id);

                if (bookCopy == null)
                {
                    return NotFound(new { Message = "Book copy does not exist" });
                }

                var isDeleted = _repo.DeleteBookCopy(id);

                if (!isDeleted)
                {
                    return BadRequest(new { Message = "Could not delte book copy" });
                }

                return Ok(new { Message = "Book copy deleted successfully" });

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Book copy Controller while trying to delete a book copy: {ex}", ex.Message);
                return BadRequest();
            }
        }
    }
}
