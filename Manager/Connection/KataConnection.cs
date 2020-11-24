using Manager.Helper;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data;
using System.Data.SqlClient;
namespace Manager.Connection
{
    public class KataConnection
    {
        public IDbConnection GetConnection()
        {
            var _connect = ReadConfig.ConnectionString("EnglishGroup");
            return (new SqlConnection(_connect));
        }
        public static QueryFactory QueryBuilder()
        {
            var connection = (new KataConnection()).GetConnection();
            var compiler = new SqlServerCompiler()
            {
                UseLegacyPagination = true
            };
            return new QueryFactory(connection, compiler);
        }
    }
}