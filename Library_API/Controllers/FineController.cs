using Library_API.Models;
using Library_API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library_API.Controllers
{
    [Route("api/fines")]
    [ApiController]
    public class FineController : ControllerBase
    {
        private readonly IFine _repo;
        private readonly ICustomer _customerRepo;
        private readonly ILogger<FineController> _logger;

        public FineController(ILogger<FineController> logger, IFine repo, ICustomer customer)
        {
            _logger = logger;
            _customerRepo = customer;
            _repo = repo;
        }

        [HttpGet]
        public ActionResult GetAllFines()
        {
            try
            {
                var fines = _repo.GetAllFines();

                if (fines == null)
                {
                    return NotFound(new {Message = "Could not find any fines" });
                }

                return Ok(fines);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Fine Controller while trying get all fines: {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("{fineid}")]
        public ActionResult GetFinesById(int fineid)
        {
            try
            {
                if (fineid <= 0)
                {
                    return BadRequest(new { Message = "Provide valid id" });
                }

                var fine = _repo.GetFinesById(fineid);


                if (fine == null)
                {
                    return NotFound();
                }

                return Ok(fine);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Fine Controller while trying :{ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("get-finebyborrowingid/{borrowingid}")]
        public ActionResult GetFineByBorrowingId(int borrowingid)
        {
            try
            {
                if (borrowingid <= 0)
                {
                    return BadRequest(new { Message = "Provide valid id" });
                }

                var fine = _repo.GetFineByBorrowingId(borrowingid);

                if (fine == null)
                {
                    return NotFound();
                }

                return Ok(fine);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Fine Controller while trying to get fine by borrowing id: {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("get-finebycustomeremail/{email}")]
        public ActionResult GetFinesByCustomerEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return BadRequest(new { Message = "Provide email" });
                }

                var fine = _repo.GetFinesByCustomerEmail(email);

                if (fine == null)
                {
                    return NotFound();
                }

                return Ok(fine);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Fine Controller while trying to get fine by customer email: {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpPut("payfine/{borrowingid}")]
        public ActionResult UpdateFineStatus(int borrowingid, string status)
        {
            try
            {
                if (borrowingid <= 0)
                {
                    return BadRequest(new { Message = "Provide valid id" });
                }

                if (string.IsNullOrWhiteSpace(status) || status.ToLower() != "paid")
                {
                    return BadRequest(new { Message = "Provide valid status" });
                }

                var isPaid = _repo.UpdateFineStatus(borrowingid, status);

                if (!isPaid)
                {
                    return BadRequest(new { Message = "Could not pay fine" });
                }

                return Ok(new {Message = "Payment captured successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Fine Controller while trying to pay fine: {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpDelete("{fineid}")]
        public ActionResult DeleteFine(int fineid)
        {
            try
            {
                if (fineid <= 0)
                {
                    return BadRequest(new { Message = "Provide valid id" });
                }

                var isDeleted = _repo.DeleteFine(fineid);

                if (!isDeleted)
                {
                    return BadRequest(new { Message = "Could not delete fine" });
                }

                return Ok(new { Message = "Fine deleted successfully" });

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Fine Controller while trying to delete fine: {ex}", ex.Message);
                return BadRequest();
            }
        }
    }
}
