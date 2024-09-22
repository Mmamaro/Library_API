using Azure.Core;
using Dapper;
using Library_API.Data;
using Library_API.Models;

namespace Library_API.Repositories
{
    public interface IBookCopy
    {
        public bool AddBookCopy(AddBookCopy request);
        public List<BookCopy> GetBookCopies();
        public BookCopy GetBookCopyById(int id);
        public BookCopy GetBookCopyByBarCode(string barcode);
        public List<BookCopy> GetBookCopiesByBookId(int id);
        public List<BookCopy> GetBookCopiesByStatus(string status);
        public bool UpdateBookCopy(int id, AddBookCopy request);
        public bool UpdateBookCopyStatus(int id, string status);
        public bool DeleteBookCopy(int id);
    }
    public class BookCopyRepo : IBookCopy
    {
        private readonly DapperContext _context;
        private readonly ILogger<BookCopyRepo> _logger;

        public BookCopyRepo(DapperContext context, ILogger<BookCopyRepo> logger)
        {
            _logger = logger;
            _context = context;
        }

        public bool AddBookCopy(AddBookCopy request)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("BookIdParam", request.BookId);
                parameters.Add("BarCodeParam", request.BarCode);
                parameters.Add("StatusParam", request.Status.ToLower());

                string sql = @"INSERT INTO [dbo].[BookCopies](BookId, BarCode, Status)
                               VALUES(@BookIdParam, @BarCodeParam , @StatusParam)";

                return _context.ExecuteSql(sql, parameters);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the BookCopy Repo while trying to add a book copy: {ex}",ex.Message);
                return false;
            }
        }

        public bool DeleteBookCopy(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CopyIdParam", id);

                string sql = "DELETE FROM [dbo].[BookCopies] WHERE CopyId = @CopyIdParam";

                return _context.ExecuteSql(sql, parameters);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the BookCopy Repo while trying to delete a book copy: {ex}", ex.Message);
                return false;
            }
        }

        public List<BookCopy> GetBookCopies()
        {
            try
            {

                string sql = "SELECT * FROM [dbo].[BookCopies]";

                return _context.QueryData<BookCopy>(sql);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the BookCopy Repo while trying to get all book copies: {ex}", ex.Message);
                return null;
            }
        }

        public List<BookCopy> GetBookCopiesByBookId(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("BookIdParam", id);

                string sql = "SELECT * FROM [dbo].[BookCopies] WHERE BookId = @BookIdParam";

                return _context.QueryDataWithParameters<BookCopy>(sql, parameters);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the BookCopy Repo while trying to get book copies by book id: {ex}", ex.Message);
                return null;
            }
        }

        public List<BookCopy> GetBookCopiesByStatus(string status)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("StatusParam", status.ToLower());

                string sql = "SELECT * FROM [dbo].[BookCopies] WHERE Status = @StatusParam";

                return _context.QueryDataWithParameters<BookCopy>(sql, parameters);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the BookCopy Repo while trying to get book copies by status: {ex}", ex.Message);
                return null;
            }
        }

        public BookCopy GetBookCopyByBarCode(string barcode)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("BarCodeParam", barcode);

                string sql = "SELECT * FROM [dbo].[BookCopies] WHERE BarCode = @BarCodeParam";

                return _context.QueryDataSingle<BookCopy>(sql, parameters);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the BookCopy Repo while trying to get book copy by barcode: {ex}", ex.Message);
                return null;
            }
        }

        public BookCopy GetBookCopyById(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CopyIdParam", id);

                string sql = "SELECT * FROM [dbo].[BookCopies] WHERE CopyId = @CopyIdParam";

                return _context.QueryDataSingle<BookCopy>(sql, parameters);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the BookCopy Repo while trying to get book copies by book id: {ex}", ex.Message);
                return null;
            }
        }

        public bool UpdateBookCopy(int id, AddBookCopy request)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CopyIdParam", id);
                parameters.Add("BookIdParam", request.BookId);
                parameters.Add("BarCodeParam", request.BarCode);
                parameters.Add("StatusParam", request.Status.ToLower());

                string sql = @"UPDATE [dbo].[BookCopies] SET";

                string sqlExtension = "";

                if (request.BookId > 0)
                {
                    sqlExtension += ", BookId = @BookIdParam";
                }

                if (!string.IsNullOrWhiteSpace(request.BarCode))
                {
                    sqlExtension += ", BarCode = @BarCodeParam";
                }

                if (!string.IsNullOrWhiteSpace(request.Status))
                {
                    sqlExtension += ", Status = @StatusParam";
                }

                string finalSql = sql + sqlExtension.Substring(1) + " WHERE CopyId = @CopyIdParam";

                return _context.ExecuteSql(finalSql, parameters);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the BookCopy Repo while trying to update a book copy: {ex}", ex.Message);
                return false;
            }
        }

        public bool UpdateBookCopyStatus(int id, string status)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CopyIdParam", id);
                parameters.Add("StatusParam", status.ToLower());

                string sql = @"UPDATE [dbo].[BookCopies] SET Status = @StatusParam WHERE CopyId = @CopyIdParam";

                return _context.ExecuteSql(sql, parameters);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the BookCopy Repo while trying to update book copy status: {ex}", ex.Message);
                return false;
            }
        }
    }
}
