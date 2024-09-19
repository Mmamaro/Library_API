using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Library_API.Data
{
    public class DapperContext
    {
        private readonly IConfiguration _config;
        private readonly ILogger<DapperContext> _logger;

        public DapperContext(IConfiguration config, ILogger<DapperContext> logger)
        {
            _config = config;
            _logger = logger;
        }

        public IEnumerable<T> QueryData<T>(string sql)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

                var data = connection.Query<T>(sql);

                if (data == null)
                {
                    return null;
                }

                return data;

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the dapper context while trying to Query data: {ex}", ex.Message);
                return default;
            }

        }
        public IEnumerable<T> QueryDataWithParameters<T>(string sql, DynamicParameters parameter)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

                var data = connection.Query<T>(sql, parameter);

                if (data == null)
                {
                    return null;
                }

                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the dapper context while trying to query data with parameters: {ex}", ex.Message);
                return default;
            }


        }

        public T? QueryDataSingle<T>(string sql, DynamicParameters parameter)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

                var data = connection.QueryFirstOrDefault<T>(sql, parameter);

                if (data == null)
                {
                    return data;
                }

                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the dapper context while trying to query single row: {ex}", ex.Message);
                return default;
            }


        }

        public bool ExecuteSql(string sql, DynamicParameters parameters)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

                return connection.Execute(sql, parameters) > 0;

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the dapper context while etrying to execute sql: {ex}", ex.Message);
                return false;
            }


        }

    }
}
