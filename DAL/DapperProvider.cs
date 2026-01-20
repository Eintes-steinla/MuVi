using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace MuVi.DAL
{
    public static class DapperProvider
    {
        /// <summary>
        /// CONNECTION STRING
        /// </summary>

        private static readonly string connectionString = @"
            Data Source=.\SQLEXPRESS;
            Initial Catalog=MuVi;
            Integrated Security=True;
            Connect Timeout=30;
            Encrypt=False;
            TrustServerCertificate=True;
            ApplicationIntent=ReadWrite;
            MultiSubnetFailover=False
        ";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// QUERY LIST
        /// SELECT * FROM TABLE
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>

        public static List<T> Query<T>(string sql, object? param = null)
        {
            using var conn = GetConnection();
            return conn.Query<T>(sql, param).AsList();
        }

        /// <summary>
        /// QUERY ONE
        /// SELECT TOP 1
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>

        public static T? QuerySingle<T>(string sql, object? param = null)
        {
            using var conn = GetConnection();
            return conn.QuerySingleOrDefault<T>(sql, param);
        }

        /// <summary>
        /// EXECUTE
        /// INSERT - UPDATE - DELETE
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>

        public static int Execute(string sql, object? param = null)
        {
            using var conn = GetConnection();
            return conn.Execute(sql, param);
        }

        /// <summary>
        /// Trả một giá trị duy nhất
        /// COUNT / SUM / MAX
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>

        public static T? ExecuteScalar<T>(string sql, object? param = null)
        {
            using var conn = GetConnection();
            return conn.ExecuteScalar<T>(sql, param);
        }

        /// <summary>
        /// gọi STORED PROCEDURE đã được tạo sẵn trong SQL Server
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procName"></param>
        /// <param name="param"></param>
        /// <returns></returns>

        public static List<T> QueryProcedure<T>(string procName, object? param = null)
        {
            using var conn = GetConnection();
            return conn.Query<T>(
                procName,
                param,
                commandType: CommandType.StoredProcedure
            ).AsList();
        }

        /// <summary>
        /// ExecuteTransaction cho phép thực thi NHIỀU câu lệnh SQL nhưng theo nguyên tắc: 
        /// tất cả thành công hoặc tất cả thất bại
        /// chạy tuần tự
        /// VD: Chuyển tiền ngân hàng:
        ///     Trừ tiền người A
        ///     Cộng tiền người B
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>

        public static bool ExecuteTransaction(Action<SqlConnection, SqlTransaction> action)
        {
            using var conn = GetConnection();
            conn.Open();

            using var tran = conn.BeginTransaction();

            try
            {
                action(conn, tran);
                tran.Commit();
                return true;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }
    }
}
