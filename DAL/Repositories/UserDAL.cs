using Dapper;
using Microsoft.Data.SqlClient;
using MuVi.DAL;
using MuVi.DTO.DTOs;
using System.Data;

namespace Muvi.DAL
{
    public class UserDAL
    {
        SqlConnection conn = DapperProvider.GetConnection();

        /// <summary>
        /// kiểm tra tên người dùng đã tồn tại chưa
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool IsUsernameExists(string username)
        {
            string sql = "SELECT COUNT(*) FROM Users WHERE Username = @Username";

            using SqlConnection conn = DapperProvider.GetConnection();

            int count = conn.ExecuteScalar<int>(sql, new { Username = username });

            return count > 0;
        }

        /// <summary>
        /// kiểm tra email đã tồn tại chưa
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool IsEmailExists(string email)
        {
            string sql = "SELECT COUNT(*) FROM Users WHERE Email = @Email";

            using SqlConnection conn = DapperProvider.GetConnection();

            int count = conn.ExecuteScalar<int>(sql, new { Email = email });

            return count > 0;
        }

        /// <summary>
        /// đăng ký người dùng mới
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool Register(UserDTO user)
        {
            string sql = @"
            INSERT INTO Users
            (
                Username,
                Password,
                Email,
                Role,
                IsActive,
                CreatedAt
            )
            VALUES
            (
                @Username,
                @Password,
                @Email,
                'User',
                1,
                GETDATE()
            )";

            int rows = conn.Execute(sql, new
            {
                user.Username,
                user.Password,
                user.Email
            });

            return rows > 0;
        }

        /// <summary>
        /// LẤY USER THEO USERNAME HOẶC EMAIL
        /// </summary>
        /// <param name="usernameOrEmail"></param>
        /// <returns></returns>
        public UserDTO? GetByUsernameOrEmail(string usernameOrEmail)
        {
            string sql = @"
            SELECT TOP 1 *
            FROM Users
            WHERE Username = @Value
               OR Email = @Value ";

            return conn.QueryFirstOrDefault<UserDTO>(sql, new
            {
                Value = usernameOrEmail
            });
        }

        /// <summary>
        /// thời gian đăng nhập cuối cùng
        /// </summary>
        /// <param name="userID></param>
        /// <returns></returns>
        public bool UpdateLastLogin(int userID)
        {
            using (var conn = DapperProvider.GetConnection())
            {
                // Câu lệnh SQL cập nhật thời gian hiện tại của hệ thống SQL Server
                string sql = "UPDATE Users SET LastLogin = GETDATE() WHERE UserId = @Id";

                // Execute trả về số dòng bị ảnh hưởng
                int rowsAffected = conn.Execute(sql, new { Id = userID });

                return rowsAffected > 0;
            }
        }
    }
}
