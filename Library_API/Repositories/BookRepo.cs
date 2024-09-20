using Dapper;
using Library_API.Data;
using Library_API.Models;

namespace Library_API.Repositories
{
    public interface IBook
    {
        public bool AddBook(AddBook request);
        public IList<Book> GetBooks();
        public Book GetBookById(int id);
        public Book GetBookByIsbn(string isbn);
        public IList<Book> GetBooksByGenreId(int Id);
        public bool UpdateBook(int Id, AddBook request);
        public bool DeleteBook(int Id);

    }
    public class BookRepo : IBook
    {
        private readonly DapperContext _context;
        private readonly ILogger<BookRepo> _logger;

        public BookRepo(DapperContext context, ILogger<BookRepo> logger)
        {
            _context = context;
            _logger = logger;
        }

        public bool AddBook(AddBook request)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("TitleParam", request.Title.ToLower());
                parameter.Add("ISBNParam", request.ISBN);
                parameter.Add("GenreIdParam", request.GenreId);

                string sql = @"INSERT INTO [dbo].[Books](Title, ISBN, GenreId) 
                               VALUES(@TitleParam, @ISBNParam, @GenreIdParam)";

                return _context.ExecuteSql(sql, parameter);

            }
            catch(Exception ex)
            {
                _logger.LogError("Error in the Book Repo while trying to add a book: {ex}",ex.Message);
                return false;
            }
        }

        public bool DeleteBook(int Id)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("IdParam", Id);

                string sql = "DELETE FROM [dbo].[Books] WHERE BookId = @IdParam";

                return _context.ExecuteSql(sql, parameter);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Book Repo while trying to delete a book: {ex}", ex.Message);
                return false;
            }
        }

        public Book GetBookById(int id)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("IdParam", id);

                string sql = "SELECT * FROM [dbo].[Books] WHERE BookId = @IdParam";

                return _context.QueryDataSingle<Book>(sql, parameter);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Book Repo while trying to get a book by id: {ex}", ex.Message);
                return null;
            }
        }

        public Book GetBookByIsbn(string isbn)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("ISBNParam", isbn);

                string sql = "SELECT * FROM [dbo].[Books] WHERE ISBN = @ISBNParam";

                return _context.QueryDataSingle<Book>(sql, parameter);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Book Repo while trying to get a book by ISBN: {ex}", ex.Message);
                return null;
            }
        }

        public IList<Book> GetBooks()
        {
            try
            {
                string sql = "SELECT * FROM [dbo].[Books]";

                return _context.QueryData<Book>(sql);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Book Repo while trying to all books: {ex}", ex.Message);
                return null;
            }
        }

        public IList<Book> GetBooksByGenreId(int Id)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("IdParam", Id);

                string sql = "SELECT * FROM [dbo].[Books] WHERE GenreId = @IdParam";

                return _context.QueryDataWithParameters<Book>(sql, parameter);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Book Repo while trying to get books by GenreId: {ex}", ex.Message);
                return null;
            }
        }

        public bool UpdateBook(int Id, AddBook request)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("IdParam", Id);
                parameter.Add("TitleParam", request.Title.ToLower());
                parameter.Add("ISBNParam", request.ISBN);
                parameter.Add("GenreIdParam", request.GenreId);

                string sql = "UPDATE [dbo].[Books] SET ";
                string sqlExtension = "";

                if (!string.IsNullOrWhiteSpace(request.Title))
                {
                    sqlExtension += ", Title = @TitleParam";
                }

                if (!string.IsNullOrWhiteSpace(request.ISBN))
                {
                    sqlExtension += ", ISBN = @ISBNParam";
                }

                if (request.GenreId > 0)
                {
                    sqlExtension += ", GenreId = @GenreIdParam";
                }

                string finalSql = sql + sqlExtension.Substring(1) + " WHERE BookId = @IdParam";

                return _context.ExecuteSql(finalSql, parameter);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Book Repo while trying to update a book: {ex}", ex.Message);
                return false;
            }
        }
    }
}
