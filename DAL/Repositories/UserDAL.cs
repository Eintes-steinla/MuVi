using Dapper;
using Microsoft.Data.SqlClient;
using MuVi.DAL;
using MuVi.DTO.DTOs;

namespace Muvi.DAL
{
    public class UserDAL
    {
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

            using SqlConnection conn = DapperProvider.GetConnection();

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

            using SqlConnection conn = DapperProvider.GetConnection();

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

        /// <summary>
        /// Lấy toàn bộ danh sách người dùng để hiển thị lên DataGrid
        /// </summary>
        public IEnumerable<UserDTO> GetAll()
        {
            string sql = "SELECT * FROM Users ORDER BY CreatedAt DESC";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<UserDTO>(sql);
        }
        public IEnumerable<UserDTO> GetUsersPaged(int pageNumber, int pageSize)
        {
            // pageNumber: Trang hiện tại (1, 2, 3...)
            // pageSize: Số dòng mỗi trang (ví dụ: 10)
            string sql = @"SELECT * FROM Users 
                   ORDER BY UserID ASC
                   OFFSET @Offset ROWS 
                   FETCH NEXT @PageSize ROWS ONLY";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<UserDTO>(sql, new
            {
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize
            });
        }

        /// <summary>
        /// Tìm kiếm người dùng theo Tên hoặc Email (Phục vụ bộ lọc ở giao diện)
        /// </summary>
        public IEnumerable<UserDTO> SearchUsers(string keyword)
        {
            string sql = @"SELECT * FROM Users 
                   WHERE Username LIKE @Key OR Email LIKE @Key 
                   ORDER BY CreatedAt DESC";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<UserDTO>(sql, new { Key = $"%{keyword}%" });
        }

        /// <summary>
        /// Cập nhật trạng thái IsActive (Khóa hoặc Mở khóa tài khoản)
        /// </summary>
        public bool UpdateStatus(int userId, bool isActive)
        {
            string sql = "UPDATE Users SET IsActive = @Status WHERE UserId = @Id";
            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new { Status = isActive, Id = userId });
            return rows > 0;
        }

        /// <summary>
        /// Xóa người dùng
        /// </summary>
        public bool Delete(int userId)
        {
            string sql = "DELETE FROM Users WHERE UserId = @Id";
            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new { Id = userId });
            return rows > 0;
        }
    }
}
