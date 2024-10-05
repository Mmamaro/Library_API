using Dapper;
using Library_API.Data;
using Library_API.Models;

namespace Library_API.Repositories
{
    public interface IBorrowing 
    { 
        public List<DetailedBorrowing> GetBorrowings();
        public List<DetailedBorrowing> GetCurrentBorrowings();
        public bool BorrowingReturn(int id, ReturnBorrowing request);
        public bool AddBorrowing(AddBorrowing request);
        public DetailedBorrowing GetBorrowingById(int id);
        public List<DetailedBorrowing> BorrowingFilters(FilterBorrowing request);
        public bool UpdateBorrowing(int id, UpdateBorrowing request);
        public bool DeleteBorrowing(int id);

    }

    public class BorrowingRepo : IBorrowing
    {
        private readonly DapperContext _context;
        private readonly ILogger<BorrowingRepo> _logger;

        public BorrowingRepo(DapperContext context, ILogger<BorrowingRepo> logger)
        {
            _context = context;
            _logger = logger;
        }

        public bool AddBorrowing(AddBorrowing request)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CustomerIdParam", request.CustomerId);
                parameters.Add("CopyIdParam", request.CopyId);
                parameters.Add("DueDateParam", request.DueDate);
                parameters.Add("BorrowDateParam", DateTime.Now);
                parameters.Add("ReturnDateParam", Convert.ToDateTime("01-01-1753 00:00:00"));
                parameters.Add("StatusParam", "borrowed");

                string sql = @"INSERT INTO [dbo].[Borrowings](CustomerId, CopyId, DueDate, BorrowDate, ReturnDate, Status)
                               VALUES(@CustomerIdParam, @CopyIdParam, @DueDateParam, @BorrowDateParam, @ReturnDateParam, @StatusParam)";

                return _context.ExecuteSql(sql, parameters);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Borrowing Repo while trying add a borrowing: {ex}",ex.Message);
                return false;
            }
        }

        public List<DetailedBorrowing> BorrowingFilters(FilterBorrowing request)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("BorrowingIdParam", request.BorrowingId);
                parameter.Add("CopyIdParam", request.CopyId);
                parameter.Add("CustomerIdParam", request.CustomerId);
                parameter.Add("startDateParam", request.startDate);
                parameter.Add("endDateParam", request.endDate);
                parameter.Add("StatusParam", request.Status.ToLower());

                string sql = @"SELECT B.BorrowingId, C.CustomerId, C.Email, Book.Title, BC.CopyId, 
                               BC.BarCode, B.BorrowDate, B.DueDate, B.ReturnDate, B.Status 
                               FROM [dbo].[Borrowings] AS B
                                 LEFT JOIN [dbo].[Customers] AS C
                                 ON B.CustomerId = C.CustomerId
                                 LEFT JOIN [dbo].[BookCopies] AS BC
                                 ON B.CopyId = BC.CopyId
                                 LEFT JOIN [dbo].[Books] AS Book
                                 ON BC.BookId = Book.BookId 
                                 WHERE";

                string sqlExtension = "";

                if (request.BorrowingId > 0)
                {
                    sqlExtension += " AND B.BorrowingId = @BorrowingIdParam";
                }

                if (request.CopyId > 0)
                {
                    sqlExtension += " AND BC.CopyId = @CopyIdParam";
                }

                if (request.CustomerId > 0)
                {
                    sqlExtension += " AND C.CustomerId = @CustomerIdParam";
                }

                if (request.startDate.HasValue)
                {
                    sqlExtension += " AND B.BorrowDate > @startDateParam";
                }

                if (request.endDate.HasValue)
                {
                    sqlExtension += " AND B.BorrowDate < @endDateParam";
                }

                if (!string.IsNullOrWhiteSpace(request.Status))
                {
                    sqlExtension += " AND B.Status = @StatusParam";
                }

                string sqlFinal = sql + sqlExtension.Substring(4);


                return _context.QueryDataWithParameters<DetailedBorrowing>(sqlFinal, parameter);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Borrowing Repo filter method: {ex}", ex.Message);
                return null;
            }
        }

        public bool BorrowingReturn(int id, ReturnBorrowing request)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("BorrowingIdParam", id);
                parameters.Add("ReturnDateParam", request.ReturnDate);
                parameters.Add("StatusParam", request.Status.ToLower());

                string sql = "UPDATE [dbo].[Borrowings] SET ReturnDate = @ReturnDateParam, Status = @StatusParam WHERE BorrowingId = @BorrowingIdParam";

                return _context.ExecuteSql(sql, parameters);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Borrowing Repo while trying return a book: {ex}", ex.Message);
                return false;
            }
        }

        public bool DeleteBorrowing(int id)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("BorrowingIdParam", id);

                string sql = "DELETE FROM [dbo].[Borrowings] WHERE BorrowingId = @BorrowingIdParam";

                return _context.ExecuteSql(sql, parameter);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Borrowing Repo while trying to delete a borrowing: {ex} ", ex.Message);
                return false;
            }
        }

        public DetailedBorrowing GetBorrowingById(int id)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("BorrowingIdParam", id);

                string sql = @"SELECT B.BorrowingId, C.CustomerId, C.Email, Book.Title, BC.CopyId, 
                               BC.BarCode, B.BorrowDate, B.DueDate, B.ReturnDate, B.Status 
                               FROM [dbo].[Borrowings] AS B
                                 LEFT JOIN [dbo].[Customers] AS C
                                 ON B.CustomerId = C.CustomerId
                                 LEFT JOIN [dbo].[BookCopies] AS BC
                                 ON B.CopyId = BC.CopyId
                                 LEFT JOIN [dbo].[Books] AS Book
                                 ON BC.BookId = Book.BookId
                                 WHERE BorrowingId = @BorrowingIdParam";

                return _context.QueryDataSingle<DetailedBorrowing>(sql, parameter);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Borrowing Repo while trying to get borrowing by id: {ex}", ex.Message);
                return null;
            }
        }

        public List<DetailedBorrowing> GetBorrowings()
        {
            try
            {

                string sql = @"SELECT B.BorrowingId, C.CustomerId, C.Email, Book.Title, BC.CopyId, 
                               BC.BarCode, B.BorrowDate, B.DueDate, B.ReturnDate, B.Status 
                               FROM [dbo].[Borrowings] AS B
                                 LEFT JOIN [dbo].[Customers] AS C
                                 ON B.CustomerId = C.CustomerId
                                 LEFT JOIN [dbo].[BookCopies] AS BC
                                 ON B.CopyId = BC.CopyId
                                 LEFT JOIN [dbo].[Books] AS Book
                                 ON BC.BookId = Book.BookId";

                return _context.QueryData<DetailedBorrowing>(sql);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Borrowing Repo while trying to get all borrowings: {ex}", ex.Message);
                return null;
            }
        }

        public List<DetailedBorrowing> GetCurrentBorrowings()
        {
            try
            {

                string sql = @"SELECT B.BorrowingId, C.CustomerId, C.Email, Book.Title, BC.CopyId, 
                               BC.BarCode, B.BorrowDate, B.DueDate, B.ReturnDate, B.Status 
                               FROM [dbo].[Borrowings] AS B
                                 LEFT JOIN [dbo].[Customers] AS C
                                 ON B.CustomerId = C.CustomerId
                                 LEFT JOIN [dbo].[BookCopies] AS BC
                                 ON B.CopyId = BC.CopyId
                                 LEFT JOIN [dbo].[Books] AS Book
                                 ON BC.BookId = Book.BookId
                                 WHERE B.Status = 'Borrowed' ";

                return _context.QueryData<DetailedBorrowing>(sql);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Borrowing Repo while trying to get all borrowings: {ex}", ex.Message);
                return null;
            }
        }

        public bool UpdateBorrowing(int id, UpdateBorrowing request)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("BorrowDateParam", request.BorrowDate);
                parameter.Add("DueDateParam", request.DueDate);
                parameter.Add("ReturnDateParam", request.ReturnDate);
                parameter.Add("BorrowingIdParam",id);

                string sql = "UPDATE [dbo].[Borrowings] SET";
                string sqlExtension = "";

                if (request.BorrowDate.HasValue)
                {
                    sqlExtension += ", BorrowDate = @BorrowDateParam";
                }
                if (request.DueDate.HasValue)
                {
                    sqlExtension += ", DueDate = @DueDateParam";
                }
                if (request.ReturnDate.HasValue)
                {
                    sqlExtension += ", ReturnDate = @ReturnDateParam";
                }

                string sqlFinal = sql + sqlExtension.Substring(1) + " WHERE BorrowingId = @BorrowingIdParam";

                return _context.ExecuteSql(sql, parameter);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Borrowing Repo while trying update a borrowing: {ex}", ex.Message);
                return false;
            }
        }
    }
}
