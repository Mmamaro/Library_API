using Library_API.Data;
using Library_API.Models;
using Library_API.Repositories;
using Library_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library_API.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomer _repo;
        private readonly ILogger<CustomerController> _logger;
        private readonly EmailService _emailService;

        public CustomerController(ICustomer repo, ILogger<CustomerController> logger, EmailService emailService)
        {
            _logger = logger;
            _repo = repo;
            _emailService = emailService;
        }

        [HttpGet]
        public ActionResult GetCustomers()
        {
            try
            {
                var customers = _repo.GetCustomers();

                if (customers == null)
                {
                    return NotFound( new {Message = "Could not get any customers"});
                }

                return Ok(customers);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Customer Controller while trying to get all customers: {ex}",ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public ActionResult GetCustomerById(int id)
        {
            try
            {
                if(id <= 0)
                {
                    return BadRequest( new {Messaage = "Provide valid id" });
                }

                var customer = _repo.GetCustomerById(id);

                if (customer == null)
                {
                    return NotFound();
                }

                return Ok(customer);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Customer Controller while trying to get customer by id: {ex}", ex.Message);
                return null;
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddCustomer(AddCustomer request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var customerExists = _repo.CustomerExists(request);

                if (customerExists != null)
                {
                    return BadRequest(new { Message = "Email/Id Number/Phone number already exists" });
                }

                var isAdded = _repo.AddCustomer(request);

                if (!isAdded)
                {
                    return BadRequest(new {Message = "Could not add customer"});
                }

                string subject = "Informatio captured on library system";
                string body = "Your information has beeen captured on the library system for future updates and due date reminders";

                await _emailService.SendEmail(request.Email, subject, body);

                return Ok(new { Message = "Customer added successfully" });

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Customer Controller while trying to add customer: {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public ActionResult UpdateCustomer(int id, UpdateCustomer request)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "Provide valid id" });
                }

                var customerExists = _repo.CustomerExistUpdate(request);

                if (customerExists != null)
                {
                    return BadRequest(new { Message = "Email/Id Number/Phone number already exists" });
                }

                var customer = _repo.GetCustomerById(id);

                if (customer == null)
                {
                    return NotFound();
                }

                var isUpdated = _repo.UpdateCustomer(id, request);

                if (!isUpdated)
                {
                    return BadRequest(new { Message = "Could not update customer" });
                }

                return Ok(new { Message = "Customer updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Customer Controller while trying to update customer: {ex} ", ex.Message);
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteCustomer(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "Provide valid id" });
                }

                var customer = _repo.GetCustomerById(id);

                if (customer == null)
                {
                    return NotFound();
                }

                var isDeleted = _repo.DeleteCustomer(id);

                if (!isDeleted)
                {
                    return BadRequest(new { Message = "Could not delete Customer" });
                }

                return Ok(new {Message = "Customer deleted successfully"});
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Customer Controller while trying to delete customer: {ex}", ex.Message);
                return BadRequest();
            }
        }
    }
}
