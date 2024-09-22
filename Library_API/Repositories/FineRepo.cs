using Azure.Core;
using Dapper;
using Library_API.Data;
using Library_API.Models;

namespace Library_API.Repositories
{
    public interface IFine
    {
        public bool AddFine(AddFine request);
        public List<DetailedFine> GetAllFines();
        public DetailedFine GetFinesById(int id);
        public DetailedFine GetFineByBorrowingId(int id);
        public DetailedFine GetUnpaidFineByCustomerId(int id);
        public DetailedFine GetFinesByCustomerEmail(string email);
        public bool UpdateFine(int id, UpdateFine request);
        public bool UpdateFineStatus(int id, string status);
        public bool DeleteFine(int id);
    }
    public class FineRepo : IFine
    {
        private readonly DapperContext _context;
        private readonly ILogger<FineRepo> _logger;

        public FineRepo(DapperContext context, ILogger<FineRepo> logger)
        {
            _context = context;
            _logger = logger;
        }

        public bool AddFine(AddFine request)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("BorrowingIdParam", request.BorrowingId);
                parameters.Add("AmountParam", request.Amount);
                parameters.Add("FineStatusParam", request.FineStatus.ToLower());

                string sql = @"INSERT INTO [dbo].[Fines](BorrowingId, Amount, FineStatus)
                               VALUES(@BorrowingIdParam, @AmountParam, @FineStatusParam)";

                return _context.ExecuteSql(sql, parameters);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Fine Repo while trying to add a fine: {ex}",ex.Message);
                return false;
            }
        }

        public bool DeleteFine(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("FineIdParam", id);

                string sql = "DELETE FROM [dbo].[Fines] WHERE FineId = @FineIdParam";

                return _context.ExecuteSql(sql, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Fine Repo while trying to delete a fine: {ex}", ex.Message);
                return false;
            }
        }

        public List<DetailedFine> GetAllFines()
        {
            try
            {

                string sql = @"SELECT F.FineId, B.BorrowingId, C.CustomerId, C.Email, F.Amount, F.FineStatus
                               FROM [dbo].[Fines] AS F
                                LEFT JOIN [dbo].[Borrowings] AS B
                                ON F.BorrowingId = B.BorrowingId
                                LEFT JOIN [dbo].[Customers] AS C
                                ON B.CustomerId = C.CustomerId";


                return _context.QueryData<DetailedFine>(sql);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Fine Repo while trying to get all fines: {ex}", ex.Message);
                return null;
            }
        }

        public DetailedFine GetFineByBorrowingId(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("BorrowingIdParam", id);

                string sql = @"SELECT F.FineId, B.BorrowingId, C.CustomerId, C.Email, F.Amount, F.FineStatus
                               FROM [dbo].[Fines] AS F
                                LEFT JOIN [dbo].[Borrowings] AS B
                                ON F.BorrowingId = B.BorrowingId
                                LEFT JOIN [dbo].[Customers] AS C
                                ON B.CustomerId = C.CustomerId
                                WHERE F.BorrowingId = @BorrowingIdParam";


                return _context.QueryDataSingle<DetailedFine>(sql, parameters);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Fine Repo while trying to get fine by borrowing id: {ex}", ex.Message);
                return null;
            }
        }

        public DetailedFine GetFinesByCustomerEmail(string email)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CustomerEmailParam", email);

                string sql = @"SELECT F.FineId, B.BorrowingId, C.CustomerId, C.Email, F.Amount, F.FineStatus
                               FROM [dbo].[Fines] AS F
                                LEFT JOIN [dbo].[Borrowings] AS B
                                ON F.BorrowingId = B.BorrowingId
                                LEFT JOIN [dbo].[Customers] AS C
                                ON B.CustomerId = C.CustomerId
                                WHERE C.Email = @CustomerEmailParam";


                return _context.QueryDataSingle<DetailedFine>(sql, parameters);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Fine Repo while trying to get fine by customer email: {ex}", ex.Message);
                return null;
            }
        }

        public DetailedFine GetFinesById(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("FineIdParam", id);

                string sql = @"SELECT F.FineId, B.BorrowingId, C.CustomerId, C.Email, F.Amount, F.FineStatus
                               FROM [dbo].[Fines] AS F
                                LEFT JOIN [dbo].[Borrowings] AS B
                                ON F.BorrowingId = B.BorrowingId
                                LEFT JOIN [dbo].[Customers] AS C
                                ON B.CustomerId = C.CustomerId
                                WHERE F.FineId = @FineIdParam";


                return _context.QueryDataSingle<DetailedFine>(sql, parameters);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Fine Repo while trying to get fine by fine id: {ex}", ex.Message);
                return null;
            }
        }

        public DetailedFine GetUnpaidFineByCustomerId(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CustomerIdParam", id);

                string sql = @"SELECT F.FineId, B.BorrowingId, C.CustomerId, C.Email, F.Amount, F.FineStatus
                               FROM [dbo].[Fines] AS F
                                LEFT JOIN [dbo].[Borrowings] AS B
                                ON F.BorrowingId = B.BorrowingId
                                LEFT JOIN [dbo].[Customers] AS C
                                ON B.CustomerId = C.CustomerId
                                WHERE C.CustomerId = @CustomerIdParam
                                AND FineStatus = 'outstanding'";


                return _context.QueryDataSingle<DetailedFine>(sql, parameters);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Fine Repo while trying to get unpaid fine: {ex}", ex.Message);
                return null;
            }
        }

        public bool UpdateFine(int id, UpdateFine request)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("FineIdParam", id);
                parameters.Add("BorrowingIdParam", request.BorrowingId);
                parameters.Add("AmountParam", request.Amount);
                parameters.Add("FineStatusParam", request.FineStatus.ToLower());

                string sql = "UPDATE [dbo].[Fines] SET";
                string sqlExtension = "";

                if (request.BorrowingId.HasValue)
                {
                    sqlExtension += ", BorrowingId = @BorrowingIdParam";
                }

                if (!string.IsNullOrWhiteSpace(request.FineStatus))
                {
                    sqlExtension += ", Amount = @AmountParam";
                }

                if (request.Amount.HasValue)
                {
                    sqlExtension += ", FineStatus = @FineStatusParam";
                }

                string sqlFinal = sql + sqlExtension.Substring(1) + " WHERE FineId = @FineIdParam";

                return _context.ExecuteSql(sqlFinal, parameters);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Fine Repo while trying to update fine: {ex}", ex.Message);
                return false;
            }
        }

        public bool UpdateFineStatus(int id, string status)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("BorrowingIdParam", id);
                parameters.Add("FineStatusParam", status.ToLower());

                string sql = "UPDATE [dbo].[Fines] SET FineStatus = @FineStatusParam WHERE BorrowingId = @BorrowingIdParam";
                
                return _context.ExecuteSql(sql, parameters);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Fine Repo while trying to update fine status: {ex}", ex.Message);
                return false;
            }
        }
    }
}
