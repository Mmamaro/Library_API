using Azure.Core;
using Library_API.Models;
using Library_API.Models.DTOs;
using Library_API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System;

namespace Library_API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _repo;
        private readonly ILogger<UserController> _logger;

        public UserController(IUser user, ILogger<UserController> logger)
        {
            _logger = logger;
            _repo = user;
        }

        [HttpGet]
        public ActionResult GetUsers()
        {
            try
            {
                var users = _repo.GetUsers();

                if (users == null)
                {
                    return NotFound(new {Message = "Could not get any users"});
                }

                List<UserDto> usersDto = users.Select(user => new UserDto
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Active = user.Active,
                    Role = user.Role
                    
                }).ToList();

                return Ok(usersDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the User Controller while : {ex}",ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public ActionResult GetUserById(int id)
        {
            try
            {
                if(id <= 0)
                {
                    return BadRequest(new { Message = "Provide valid id" });
                }

                var user = _repo.GetUserById(id);

                if (user == null)
                {
                    return NotFound();
                }

                var userDto = new UserDto()
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Active = user.Active,
                    Role = user.Role    
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the User Controller while to get user by id : {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("get-userbyemail/{email}")]
        public ActionResult UserExists(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return BadRequest(new { Message = "Provide email" });
                }

                var user = _repo.UserExists(email);

                if (user == null)
                {
                    return NotFound();
                }

                var userDto = new UserDto()
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Active = user.Active,
                    Role = user.Role
                };

                return Ok(userDto);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the User Controller while trying to get user by email: {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpPost]
        public ActionResult AddUser(AddUser request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userExists = _repo.UserExists(request.Email);

                if (userExists != null)
                {
                    return BadRequest(new { Message = "User email already exists" });
                }

                var user = new User() 
                { 
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Password = request.Password,
                    Active = true,
                    isFirstSignIn = true,
                    isTwoFaVerified = false,
                    ManualCode = "dfgr",
                    Role = "user",
                    QRCode = "dgfer",
                    twoFaKey = "dwfb"
                };

                var isAdded = _repo.AddUser(user);

                if (!isAdded)
                {
                    return BadRequest(new { Message = "Could not add user" });
                }

                return Ok(new { Message = "User registered successfully" });

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the User Controller while trying to register user: {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public ActionResult UpdateUser(int id, UpdateUser request)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "Provide valid id" });
                }

                var user = _repo.UserExists(request.Email);

                if (user != null)
                {
                    return BadRequest(new { Message = "User email already exists" });
                }

                var isUpdated = _repo.UpdateUser(id, request);

                if (!isUpdated)
                {
                    return BadRequest(new { Message = "Could not update user" });
                }

                return Ok(new { Message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the User Controller while trying to update user: {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpPut("update-userrole{id}/{role}")]
        public ActionResult UpdateUserRole(int id, string role)
        {
            try
            {
                List<string> validRoles = new List<string> { "user", "admin"};

                if (id <= 0)
                {
                    return BadRequest(new { Message = "Provide valid id" });
                }

                if (string.IsNullOrWhiteSpace(role) || !validRoles.Contains(role.ToLower()))
                {
                    return BadRequest(new { Message = "Provide valid role" });
                }

                var isUpdated = _repo.UpdateUserRole(id, role.ToLower());

                if (!isUpdated)
                {
                    return BadRequest(new { Message = "Could not update user role" });
                }

                return Ok(new { Message = "User role updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the User Controller while trying to update user role: {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteUser(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "Provide valid id" });
                }

                var isDeleted = _repo.DeleteUser(id);

                if (!isDeleted)
                {
                    return BadRequest(new { Message = "Could not delete user" });
                }

                return Ok(new { Message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the User Controller while trying to delete user : {ex}", ex.Message);
                return BadRequest();
            }
        }
    }
}
