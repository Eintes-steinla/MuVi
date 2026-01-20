using Microsoft.Data.SqlClient;
using System.Data;

namespace Muvi.DAL
{
    public static class DataProvider
    {
        /// <summary>
        /// Chuỗi kết nối tới csdl MuVi
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
        /// Đọc dữ liệu từ CSDL
        /// slect, tìm kiếm, lọc dữ liệu, hiển thị grid, load combobox, load datatable
        /// Hàm thực thi truy vấn lấy dữ liệu (SELECT) trả về DataTable
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            DataTable data = new DataTable();
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(data);
                conn.Close();
            }
            return data;
        }

        /// <summary>
        /// Ghi dữ liệu vào CSDL
        /// Hàm thực thi lệnh (INSERT, UPDATE, DELETE) trả về số dòng bị tác động
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            int data = 0;
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                data = command.ExecuteNonQuery();   
                conn.Close();
            }
            return data;
        }

        /// <summary>
        /// SELECT 1 GIÁ TRỊ
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            object result;

            using (SqlConnection conn = GetConnection())
            {
                conn.Open();

                SqlCommand command = new SqlCommand(query, conn);

                if (parameters != null)
                    command.Parameters.AddRange(parameters);

                result = command.ExecuteScalar();
            }

            return result;
        }

        /// <summary>
        /// Đọc dữ liệu nhiều dòng
        /// xuất Excel, import dữ liệu, xử lý hàng trăm nghìn bản ghi, background process, không cần DataTable
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string query, SqlParameter[] parameters = null)
        {
            SqlConnection conn = GetConnection();
            conn.Open();

            SqlCommand command = new SqlCommand(query, conn);

            if (parameters != null)
                command.Parameters.AddRange(parameters);

            // Khi reader đóng → connection tự đóng
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }
    }
}
