using Google.Authenticator;
using Library_API.Models;
using Library_API.Repositories;
using Library_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Library_API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuth _repo;
        private readonly TokenService _tokenService;
        private readonly IUser _userRepo;
        private readonly TwoFaService _twoFaService;
        private readonly ILogger<AuthController> _logger;
        private readonly EmailService _emailService;

        public AuthController(IAuth repo,TokenService tokenService, ILogger<AuthController> logger, 
            IUser userRepo, TwoFaService twoFaService, EmailService emailService)
        {
            _repo = repo;
            _logger = logger;
            _tokenService = tokenService;
            _userRepo = userRepo;
            _twoFaService = twoFaService;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> AddUser(AddUser request)
        {
            try
            {

                var userExists = _userRepo.UserExists(request.Email);

                if (userExists != null)
                {
                    return BadRequest(new { Message = "User email already exists" });
                }

                var twoFaSetup = _twoFaService.TwoFASetup(request.Email);

                var user = new User()
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Password = request.Password,
                    Active = true,
                    isFirstSignIn = true,
                    isTwoFaVerified = false,
                    ManualCode = twoFaSetup.ManualEntryCode,
                    Role = "user",
                    QRCode = twoFaSetup.QrCodeUrl,
                    twoFaKey = twoFaSetup.TwoFAKey
                };

                var isAdded = _repo.AddUser(user);

                if (!isAdded)
                {
                    return BadRequest(new { Message = "Could not add user" });
                }

                await _emailService.SendEmail(request.Email, "Registered on the library system", "Your registration was successfull");

                return Ok(new { Message = "User registered successfully" });

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the User Controller while trying to register user: {ex}", ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("normal-login")]
        public ActionResult Login(LoginModel request)
        {
            try
            {

                var user = _repo.Login(request);

                if (user == null)
                {
                    return Unauthorized();
                }

                if (user.Active == false)
                {
                    return Unauthorized();
                }

                if (user == null)
                {
                    return Unauthorized(new {Message = "Invalid Credentials" });
                }
                var token = _tokenService.GenerateLoginToken(user);

                var credentials = new Credentials()
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = user.Role,
                    Active = user.Active,
                    isFirstSignIn = user.isFirstSignIn,
                    isTwoFaVerified = user.isTwoFaVerified,
                    ManualCode = user.ManualCode,
                    QRCode = user.QRCode,
                    Token = token
                };

                return Ok(credentials);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Auth Controller while trying to login: {ex}", ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpPost("twofa-login")]
        public ActionResult TwoFaLogIn(TwoFALoginModel request)
        {
            try
            {
                var user = _userRepo.UserExists(request.Email);

                if(user == null)
                {
                    return Unauthorized(new {Messaage = "Invalid Email"});
                }

                byte[] encryptedBytes = Convert.FromBase64String(user.twoFaKey);
                string decryptedSecret = _twoFaService.DecryptString(encryptedBytes);

                TwoFactorAuthenticator twoFactorAuthenticator = new TwoFactorAuthenticator();
                var isValid = twoFactorAuthenticator.ValidateTwoFactorPIN(decryptedSecret, request.Code);

                if(!isValid)
                {
                    return Unauthorized();
                }

                var isUpdated = _repo.UpdateFirstSignIn(user.Email);

                if (!isUpdated)
                {
                    return BadRequest();
                }

                var updatedUser = _userRepo.UserExists(user.Email);

                var token = _tokenService.GenerateLoginToken(updatedUser);

                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized();
                }

                var credentials = new Credentials() 
                { 
                    UserId = updatedUser.UserId,
                    FirstName = updatedUser.FirstName,
                    LastName = updatedUser.LastName,
                    Email = updatedUser.Email,
                    Role = updatedUser.Role,
                    isFirstSignIn = updatedUser.isFirstSignIn,
                    isTwoFaVerified = updatedUser.isTwoFaVerified,
                    ManualCode = updatedUser.ManualCode,
                    QRCode = updatedUser.QRCode,
                    Token = token,
                    Active = true,
                };

                return Ok(credentials);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Auth Controller while trying to login with Two Fa: {ex}", ex.Message);
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpGet("forgot-password")]
        public async Task<ActionResult> forgotPassword(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return BadRequest(new { Messagee = "Provide valid email" });
                }

                var user = _userRepo.UserExists(email);

                if (user == null)
                {
                    return NotFound(new { Messagee = "User does not exist" });
                }

                var token = _tokenService.GenerateForgotPasswordToken(user);

                //The token will have to be copied and and put on authorization to use it 
                //but if I had front end I would just append the token to the front end url and front end would take care of the request
                string url = $"https://localhost:7239/api/Auth/change-password?token={token}";

                string body = $"Update your password here: {url}";

                await _emailService.SendEmail(user.Email, "Change Password", body);

                return Ok(new {Message = "Email sent succeessfully"});

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Auth Controller in the forgot password method: {ex}", ex.Message);
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword(LoginModel request)
        {
            try
            {

                var currentUser = User.FindFirst(ClaimTypes.Email)?.Value;

                var user = _userRepo.UserExists(request.Email);

                if (user == null)
                {
                    return NotFound(new { Messagee = "User does not exist" });
                }

                if (currentUser != user.Email)
                {
                    return Unauthorized(new { Messagee = "Different user trying to change password" });
                }

                var isUpdated = _repo.UpdatePassword(request);

                if (!isUpdated)
                {
                    return BadRequest(new { Messagee = "Could not update paassword" });
                }

                string subject = "Password updated successfully";
                string body = "Your library system paassowrd was updated successfully";

                await _emailService.SendEmail(user.Email, subject, body);

                return Ok( new {Message = "Password updated successfully"});

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Auth Controller in the change password method: {ex}", ex.Message);
                return BadRequest();
            }
        }
    }
}
