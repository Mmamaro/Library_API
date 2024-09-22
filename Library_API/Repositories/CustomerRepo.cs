using Azure.Core;
using Dapper;
using Library_API.Data;
using Library_API.Models;

namespace Library_API.Repositories
{
    public interface ICustomer
    {
        public List<Customer> GetCustomers();
        public Customer GetCustomerById(int id);
        public Customer CustomerExists(AddCustomer request);
        public Customer CustomerExistUpdate(UpdateCustomer request);
        public bool AddCustomer(AddCustomer request);
        public bool UpdateCustomer(int id, UpdateCustomer request);
        public bool DeleteCustomer(int id);
    }
    public class CustomerRepo : ICustomer
    {
        private readonly DapperContext _context;
        private readonly ILogger<CustomerRepo> _logger;

        public CustomerRepo(DapperContext context, ILogger<CustomerRepo> logger)
        {
            _context = context;
            _logger = logger;
        }
        public bool AddCustomer(AddCustomer request)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("FirstNameParam", request.FirstName);
                parameters.Add("LastNameParam", request.LastName);
                parameters.Add("IdNumberParam", request.IdNumber);
                parameters.Add("EmailParam", request.Email);
                parameters.Add("PhoneParam", request.Phone);

                string sql = @"INSERT INTO [dbo].[Customers](FirstName, LastName, IdNumber, Email, Phone)
                               VALUES(@FirstNameParam, @LastNameParam, @IdNumberParam, @EmailParam, @PhoneParam)";

                return _context.ExecuteSql(sql, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Customer Repo while trying to add a customer: {ex}",ex.Message);
                return false;
            }
        }

        public Customer CustomerExists(AddCustomer request)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("IdNumberParam", request.IdNumber);
                parameters.Add("EmailParam", request.Email);
                parameters.Add("PhoneParam", request.Phone);

                string sql = @"SELECT * FROM [dbo].[Customers] WHERE IdNumber = @IdNumberParam OR Email = @EmailParam OR Phone = @PhoneParam";

                return _context.QueryDataSingle<Customer>(sql, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Customer Repo while trying to check if customer already exists: {ex}", ex.Message);
                return null;
            }
        }

        public Customer CustomerExistUpdate(UpdateCustomer request)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("IdNumberParam", request.IdNumber);
                parameters.Add("EmailParam", request.Email);
                parameters.Add("PhoneParam", request.Phone);

                string sql = @"SELECT * FROM [dbo].[Customers] WHERE IdNumber = @IdNumberParam OR Email = @EmailParam OR Phone = @PhoneParam";

                return _context.QueryDataSingle<Customer>(sql, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Customer Repo while trying to check if customer already exists: {ex}", ex.Message);
                return null;
            }
        }

        public bool DeleteCustomer(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("IdParam", id);

                string sql = "DELETE FROM [dbo].[Customers] WHERE CustomerId = @IdParam";

                return _context.ExecuteSql(sql, parameters);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Customer Repo while trying to delete a customer: {ex}", ex.Message);
                return false;
            }
        }

        public Customer GetCustomerById(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("IdParam", id);

                string sql = "SELECT * FROM [dbo].[Customers] WHERE CustomerId = @IdParam";

                return _context.QueryDataSingle<Customer>(sql, parameters);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Customer Repo while trying to get customer by id: {ex}", ex.Message);
                return null;
            }
        }

        public List<Customer> GetCustomers()
        {
            try
            {
                string sql = "SELECT * FROM [dbo].[Customers]";

                return _context.QueryData<Customer>(sql);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Customer Repo while trying to get all customers: {ex}", ex.Message);
                return null;
            }
        }

        public bool UpdateCustomer(int id, UpdateCustomer request)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("IdParam", id);
                parameters.Add("FirstNameParam", request.FirstName);
                parameters.Add("LastNameParam", request.LastName);
                parameters.Add("IdNumberParam", request.IdNumber);
                parameters.Add("EmailParam", request.Email);
                parameters.Add("PhoneParam", request.Phone);

                string sql = @"UPDATE [dbo].[Customers] SET";
                string sqlExtension = "";

                if (!string.IsNullOrWhiteSpace(request.FirstName))
                {
                    sqlExtension += ", FirstName = @FirstNameParam";
                }
                if (!string.IsNullOrWhiteSpace(request.LastName))
                {
                    sqlExtension += ", LastName = @LastNameParam";
                }
                if (!string.IsNullOrWhiteSpace(request.IdNumber))
                {
                    sqlExtension += ", IdNumber = @IdNumberParam";
                }
                if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    sqlExtension += ", Email = @EmailParam";
                }
                if (!string.IsNullOrWhiteSpace(request.Phone))
                {
                    sqlExtension += ", Phone = @PhoneParam";
                }

                string finalSql = sql + sqlExtension.Substring(1) + " WHERE CustomerId = @IdParam";

                return _context.ExecuteSql(finalSql, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Customer Repo while trying to update a customer: {ex}", ex.Message);
                return false;
            }
        }
    }
}
