using Library_API.Models;
using Library_API.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Library_API.Services
{
    public class TokenService
    {
        private readonly ILogger<TokenService> _logger;
        private readonly IConfiguration _config;

        public TokenService(ILogger<TokenService> logger, IConfiguration configuration)
        {
            _config = configuration;
            _logger = logger;
        }

        public string? GenerateLoginToken(User user)
        {
            try
            {

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);

                var cliams = new List<Claim>
                {
                    new Claim(ClaimTypes.PrimarySid, user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var tokenDesciptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(cliams),
                    IssuedAt = DateTime.UtcNow,
                    Expires = DateTime.UtcNow.AddHours(12),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDesciptor);
                var tokenString = tokenHandler.WriteToken(token);

                return tokenString;

            }catch (Exception ex)
            {
                _logger.LogError("Error in the token service while trying to generate login token: {ex}",ex.Message);
                return null;
            }
        }

        public string? GenerateForgotPasswordToken(User user)
        {
            try
            {

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);

                var cliams = new List<Claim>
                {
                    new Claim(ClaimTypes.Sid, user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                var tokenDesciptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(cliams),
                    IssuedAt = DateTime.UtcNow,
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDesciptor);
                var tokenString = tokenHandler.WriteToken(token);

                return tokenString;

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the token service while trying to generate login token: {ex}", ex.Message);
                return null;
            }
        }
    }
}
