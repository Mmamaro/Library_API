using Azure.Core;
using BCrypt.Net;
using Dapper;
using Library_API.Data;
using Library_API.Models;

namespace Library_API.Repositories
{
    public interface IUser
    {
        public List<User> GetUsers();
        public User GetUserById(int id);
        public User UserExists(string email);
        public bool AddUser(User request);
        public bool UpdateUserRole(int id, string role);
        public bool UpdateUser(int id, UpdateUser request);
        public bool DeleteUser(int id);

    }
    public class UserRepo : IUser
    {
        public readonly DapperContext _context;
        private readonly ILogger<UserRepo> _logger;

        public UserRepo(ILogger<UserRepo> logger, DapperContext context)
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


            } catch (Exception ex)
            {
                _logger.LogError("Error in the User Repo while trying to add user: {ex}",ex.Message);
                return false;
            }
        }

        public bool DeleteUser(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UserIdParam", id);

                string sql = "DELETE FROM [dbo].[Users] WHERE UserId = @UserIdParam";

                return _context.ExecuteSql(sql, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the User Repo while trying to delete user: {ex}", ex.Message);
                return false;
            }
        }

        public User GetUserById(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UserIdParam", id);

                string sql = "SELECT * FROM [dbo].[Users] WHERE UserId = @UserIdParam";

                return _context.QueryDataSingle<User>(sql, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the User Repo while trying to get user by id: {ex}", ex.Message);
                return null;
            }
        }

        public List<User> GetUsers()
        {
            try
            {
                string sql = "SELECT * FROM [dbo].[Users]";

                return _context.QueryData<User>(sql);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the User Repo while trying to get all users: {ex}", ex.Message);
                return null;
            }
        }

        public bool UpdateUser(int id, UpdateUser request)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UserIdParam", id);
                parameters.Add("@FirstNameParam", request.FirstName);
                parameters.Add("@LastNameParam", request.LastName);
                parameters.Add("@EmailParam", request.Email.ToLower());

                string sql = @"UPDATE [dbo].[Users] SET";
                string sqlExtension = "";

                if(!string.IsNullOrWhiteSpace(request.Email))
                {
                    sqlExtension += ", Email = @EmailParam";
                }

                if(!string.IsNullOrWhiteSpace(request.FirstName))
                {
                    sqlExtension += ", FirstName = @FirstNameParam";
                }

                if(!string.IsNullOrWhiteSpace(request.LastName))
                {
                    sqlExtension += ", LastName = @LastNameParam";
                }

                string sqlFinal = sql + sqlExtension.Substring(1) + " WHERE UserId = @UserIdParam";

                return _context.ExecuteSql(sqlFinal, parameters);


            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the User Repo while trying to update user: {ex}", ex.Message);
                return false;
            }
        }

        public bool UpdateUserRole(int id, string role)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UserIdParam", id);
                parameters.Add("@RoleParam", role.ToLower());

                string sql = @"UPDATE [dbo].[Users] SET Role = @RoleParam WHERE UserId = @UserIdParam";

                return _context.ExecuteSql(sql, parameters);


            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the User Repo while trying to update user role: {ex}", ex.Message);
                return false;
            }
        }

        public User UserExists(string email)
        {
            try
            {

                var parameters = new DynamicParameters();
                parameters.Add("@EmailParam", email.ToLower());

                string sql = "SELECT * FROM [dbo].[Users] WHERE Email = @EmailParam";

                return _context.QueryDataSingle<User>(sql, parameters);
                
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the User Repo whilee trying to check if user exists using their email: {ex}", ex.Message);
                return null;
            }
        }
    }
}
