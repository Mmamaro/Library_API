using Azure.Core;
using Dapper;
using Library_API.Data;
using Library_API.Models;
using static QRCoder.PayloadGenerator;

namespace Library_API.Repositories
{
    public interface IAuth
    {
        public User Login(LoginModel request);
        public User TwoFaKeyExists(string twoFaKey);
        public bool AddUser(User request);
        public bool UpdateFirstSignIn(string email);
        public bool UpdatePassword(LoginModel request);

    }
    public class AuthRepo : IAuth
    {
        private readonly DapperContext _context;
        private readonly ILogger<AuthRepo> _logger;

        public AuthRepo(DapperContext context, ILogger<AuthRepo> logger)
        {
            _context = context;
            _logger = logger;
        }


        public bool AddUser(User request)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@FirstNameParam", request.FirstName);
                parameters.Add("@LastNameParam", request.LastName);
                parameters.Add("@EmailParam", request.Email.ToLower());
                parameters.Add("@PasswordParam", BCrypt.Net.BCrypt.HashPassword(request.Password));
                parameters.Add("@RoleParam", request.Role);
                parameters.Add("@isFirstSigninParam", request.isFirstSignIn);
                parameters.Add("@isTwoFaVerifiedParam", request.isTwoFaVerified);
                parameters.Add("@twoFaKeyParam", request.twoFaKey);
                parameters.Add("@QRCodeParam", request.QRCode);
                parameters.Add("@ManualCodeParam", request.ManualCode);
                parameters.Add("@ActiveParam", request.Active);

                string sql = @"INSERT INTO [dbo].[Users](FirstName, LastName, Email, Password, Role, isFirstSignin, isTwoFaVerified,
                                                        twoFaKey, QRCode, ManualCode, Active)
                               VALUES(@FirstNameParam, @LastNameParam, @EmailParam, @PasswordParam, @RoleParam, @isFirstSigninParam,
                                       @isTwoFaVerifiedParam, @twoFaKeyParam, @QRCodeParam, @ManualCodeParam, @ActiveParam)";

                return _context.ExecuteSql(sql, parameters);


            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the User Repo while trying to add user: {ex}", ex.Message);
                return false;
            }
        }


        public User Login(LoginModel request)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Email", request.Email);

                string sql = "SELECT * FROM [dbo].[Users] WHERE Email = @Email";

                var user = _context.QueryDataSingle<User>(sql, parameters);

                if (user == null)
                {
                    return null;
                }

                var isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

                if (!isValid)
                {
                    return null;
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Auth Repo while trying to log in user: {ex}",ex.Message);
                return null;
            }
        }

        public User TwoFaKeyExists(string twoFaKey)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("twoFaKey", twoFaKey);

                string sql = "SELECT * FROM [dbo].[Users] WHERE twoFaKey = @twoFaKey";

                return _context.QueryDataSingle<User>(sql, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Auth Repo while trying to check if twoFaKey already exists: {ex}", ex.Message);
                return null;
            }
        }

        public bool UpdateFirstSignIn(string email)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Email", email);
                parameters.Add("isFirstSignin", false);
                parameters.Add("isTwoFaVerified", true);

                string sql = "UPDATE [dbo].[Users] SET isFirstSignin = @isFirstSignin, isTwoFaVerified = @isTwoFaVerified WHERE Email = @Email";

                return _context.ExecuteSql(sql, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Auth Repo while trying to update first sign in: {ex}", ex.Message);
                return false;
            }
        }

        public bool UpdatePassword(LoginModel request)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Email", request.Email);
                parameters.Add("Password", BCrypt.Net.BCrypt.HashPassword(request.Password));

                string sql = "UPDATE [dbo].[Users] SET Password = @Password WHERE Email = @Email";

                return _context.ExecuteSql(sql, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Auth Repo while trying to update user password: {ex}", ex.Message);
                return false;
            }
        }
    }
}
