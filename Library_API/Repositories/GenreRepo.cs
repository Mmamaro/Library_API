using Dapper;
using Library_API.Data;
using Library_API.Models;

namespace Library_API.Repositories
{
    public interface IGenre
    {
        public List<Genre> GetGenres();
        public Genre GetGenreById(int id);
        public Genre GetGenreByName(string name);
        public bool AddGenre(AddGenre request);
        public bool UpdateGenre(int Id, AddGenre request);
        public bool DeleteGenre(int id);
    }

    public class GenreRepo : IGenre
    {
        private readonly DapperContext _context;
        private readonly ILogger<GenreRepo> _logger;

        public GenreRepo(DapperContext context, ILogger<GenreRepo> logger)
        {
            _context = context;
            _logger = logger;
        }

        public bool AddGenre(AddGenre request)
        {
            try
            {

                var parameter = new DynamicParameters();
                parameter.Add("GenreNameParam", request.GenreName.ToLower());

                string sql = "INSERT INTO [dbo].[Genres](GenreName) VALUES(@GenreNameParam)";

                return _context.ExecuteSql(sql, parameter);


            } catch(Exception ex) 
            { 
                 
                _logger.LogError("Error in the genre repo while trying to add a genre: {ex}", ex.Message);
                return false;
            }
        }

        public bool DeleteGenre(int id)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("IdParam", id);

                string sql = "DELETE FROM [dbo].[Genres] WHERE GenreId = @IdParam";

                return _context.ExecuteSql(sql, parameter);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the genre repo while trying to delete a genre: {ex}", ex.Message);
                return false;
            }
        }

        public Genre GetGenreById(int id)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("IdParam", id);

                string sql = "SELECT * FROM [dbo].[Genres] WHERE GenreId = @IdParam";

                return _context.QueryDataSingle<Genre>(sql, parameter);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the genre repo while trying to get a genre by id: {ex}", ex.Message);
                return null;
            }
        }

        public Genre GetGenreByName(string name)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("GenreNameParam", name.ToLower());

                string sql = "SELECT * FROM [dbo].[Genres] WHERE GenreName = @GenreNameParam";

                return _context.QueryDataSingle<Genre>(sql, parameter);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the genre repo while trying to get a genre by name: {ex}", ex.Message);
                return null;
            }
        }

        public List<Genre> GetGenres()
        {
            try
            {
                string sql = "SELECT * FROM [dbo].[Genres] ORDER BY GenreId ASC";

                return _context.QueryData<Genre>(sql);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the genre repo while trying to get all genres: {ex}", ex.Message);
                return null;
            }
        }

        public bool UpdateGenre(int Id, AddGenre request)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("GenreIdParam", Id);
                parameter.Add("GenreNameParam", request.GenreName.ToLower());

                string sql = "UPDATE [dbo].[Genres] SET GenreName = @GenreNameParam  WHERE GenreId = @GenreIdParam";

                return _context.ExecuteSql(sql, parameter);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the genre repo while trying to update a genre: {ex}", ex.Message);
                return false;
            }
        }
        }
}
