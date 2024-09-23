using Azure.Core;
using Library_API.Models;
using Library_API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library_API.Controllers
{
    [Route("api/borrowings")]
    [ApiController]
    public class BorrowingController : ControllerBase
    {
        private readonly IBorrowing _repo;
        private readonly IFine _fineRepo;
        private readonly ILogger<BorrowingController> _logger;
        private readonly IBookCopy _bookCopyRepo;
        private readonly ICustomer _customerRepo;

        public BorrowingController(ILogger<BorrowingController> logger, IBorrowing borrowing, 
            IFine fine, IBookCopy bookCopyRepo, ICustomer customer)
        {
            _logger = logger;
            _fineRepo = fine;
            _repo = borrowing;
            _bookCopyRepo = bookCopyRepo;
            _customerRepo = customer;
        }

        [HttpGet]
        public ActionResult GetBorrowings()
        {
            try
            {
                var borrowings = _repo.GetBorrowings();

                if(borrowings == null)
                {
                    return NotFound( new {Message = "Could not get any borrowings"});
                }

                return Ok(borrowings);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Borrowing Controller while trying to Get all borrowings: {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpPost("filters")]
        public ActionResult BorrowingFilters(FilterBorrowing request)
        {
            try
            {
                var borrowings = _repo.BorrowingFilters(request);

                if (borrowings.Count() == 0)
                {
                    return NotFound();
                }

                return Ok(borrowings);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Borrowing Controller while trying to get borrowings by filters: {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpPut("borrowing-return/{borrowingid}")]
        public ActionResult BorrowingReturn(int borrowingid, ReturnBorrowing request)
        {
            try
            {
                List<string> validStatus = new List<string> { "lost", "returned" };

                if (borrowingid <= 0)
                {
                    return BadRequest(new { Message = "Provide valid id" });
                }

                if (string.IsNullOrWhiteSpace(request.Status))
                {
                    return BadRequest(new { Message = "Provide status" });
                }

                if (!validStatus.Contains(request.Status.ToLower()))
                {
                    return BadRequest(new { Message = "Invalid status" });
                }

                var borrowing = _repo.GetBorrowingById(borrowingid);

                if (borrowing == null)
                {
                    return NotFound(new { Message = "Borrowing id does not exist" });
                }

                if (request.Status.ToLower() == "lost")
                {
                    var fine = new AddFine() 
                    { 
                        BorrowingId = borrowingid,
                        Amount = 300,
                        FineStatus = "outstanding"
                    };

                    var fineAdded = _fineRepo.AddFine(fine);

                    if (!fineAdded)
                    {
                        return BadRequest(new { Message = "Could not add fine" });
                    }

                    var updateBookCopyStatus = _bookCopyRepo.UpdateBookCopyStatus(borrowing.CopyId, "lost");

                    if (!updateBookCopyStatus)
                    {
                        return BadRequest(new { Message = "Could not update book copy status" });
                    }
                }

                if (request.Status.ToLower() != "lost")
                {
                    if (request.ReturnDate > borrowing.DueDate)
                    {
                        var fine = new AddFine()
                        {
                            BorrowingId = borrowingid,
                            Amount = 100,
                            FineStatus = "outstanding"
                        };

                        var fineAdded = _fineRepo.AddFine(fine);

                        if (!fineAdded)
                        {
                            return BadRequest(new { Message = "Could not add fine" });
                        }
                    }
                }

                if (request.Status.ToLower() == "returned" )
                {

                    var updateBookCopyStatus = _bookCopyRepo.UpdateBookCopyStatus(borrowing.CopyId, "available");

                    if (!updateBookCopyStatus)
                    {
                        return BadRequest(new { Message = "Could not update book copy status" });
                    }
                }

                var isReturned = _repo.BorrowingReturn(borrowingid, request);

                if (!isReturned)
                {
                    return BadRequest(new { Message = "Could not return book" });
                }

                return Ok(new { Message = "Book returned successfully" });

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Borrowing Controller while trying to return a book: {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpPost("addborrowing")]
        public ActionResult AddBorrowing(AddBorrowing request)
        {
            try
            {
                if (request.CopyId <= 0 || request.CustomerId <= 0)
                {
                    return BadRequest(new { Message = "Provide valid ids" });
                }

                if (request.DueDate < DateTime.Now || request.DueDate == DateTime.Today)
                {
                    return BadRequest(new { Message = "Due date cannot be in the past or present it must be a future date like tomorrow" });
                }

                var copy = _bookCopyRepo.GetBookCopyById(request.CopyId);

                if (copy == null)
                {
                    return NotFound(new { Message = "Book copy does not exist" });
                }

                if (copy.Status == "borrowed" || copy.Status == "lost")
                {
                    return NotFound(new { Message = "Book copy is either borrowed or lost" });
                }

                var customer = _customerRepo.GetCustomerById(request.CustomerId);

                if (customer == null)
                {
                    return NotFound(new { Message = "Customer does not exist" });
                }

                var unpaidFines = _fineRepo.GetUnpaidFineByCustomerId(request.CustomerId);

                if (unpaidFines != null)
                {
                    return BadRequest(new { Message = "Customer has unpaid fines" });
                }

                var isAdded = _repo.AddBorrowing(request);

                if (!isAdded)
                {
                    return BadRequest(new { Message = "Could not add borrowing" });
                }

                var updateBookCopyStatus = _bookCopyRepo.UpdateBookCopyStatus(request.CopyId, "borrowed");

                if (!updateBookCopyStatus)
                {
                    return BadRequest(new { Message = "Could not update book copy status" });
                }

                return Ok(new { Message = "Borrowing was successful" });

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Borrowing Controller while trying add borrowing: {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpDelete("{borrowingid}")]
        public ActionResult DeleteBorrowing(int borrowingid)
        {
            try
            {
                if (borrowingid <= 0)
                {
                    return BadRequest(new { Message = "Provide valid id" });
                }

                var isDeleted = _repo.DeleteBorrowing(borrowingid);

                if (!isDeleted)
                {
                    return BadRequest(new { Message = "Could not delete borrowing" });
                }

                return Ok(new { Message = "Borrowing deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Borrowing Controller while trying delete borrowing: {ex}", ex.Message);
                return BadRequest();
            }
        }
    }
}
