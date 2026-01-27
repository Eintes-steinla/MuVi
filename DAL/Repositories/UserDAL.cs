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
        /// Thêm người dùng mới với đầy đủ thông tin
        /// </summary>
        public bool AddUser(UserDTO user)
        {
            string sql = @"
            INSERT INTO Users
            (
                Username,
                Password,
                Email,
                DateOfBirth,
                Avatar,
                Role,
                IsActive,
                CreatedAt,
                UpdatedAt
            )
            VALUES
            (
                @Username,
                @Password,
                @Email,
                @DateOfBirth,
                @Avatar,
                @Role,
                @IsActive,
                GETDATE(),
                GETDATE()
            )";

            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new
            {
                user.Username,
                user.Password,
                user.Email,
                user.DateOfBirth,
                user.Avatar,
                user.Role,
                user.IsActive
            });

            return rows > 0;
        }

        /// <summary>
        /// Cập nhật thông tin người dùng
        /// </summary>
        public bool UpdateUser(UserDTO user)
        {
            string sql = @"
            UPDATE Users 
            SET 
                Username = @Username,
                Email = @Email,
                DateOfBirth = @DateOfBirth,
                Avatar = @Avatar,
                Role = @Role,
                IsActive = @IsActive,
                UpdatedAt = GETDATE()
            WHERE UserID = @UserID";

            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new
            {
                user.UserID,
                user.Username,
                user.Email,
                user.DateOfBirth,
                user.Avatar,
                user.Role,
                user.IsActive
            });

            return rows > 0;
        }

        /// <summary>
        /// LẤY USER THEO USERNAME HOẶC EMAIL
        /// </summary>
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
        /// Lấy user theo ID
        /// </summary>
        public UserDTO? GetById(int userId)
        {
            string sql = "SELECT * FROM Users WHERE UserID = @UserId";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.QueryFirstOrDefault<UserDTO>(sql, new { UserId = userId });
        }

        /// <summary>
        /// thời gian đăng nhập cuối cùng
        /// </summary>
        public bool UpdateLastLogin(int userID)
        {
            using (var conn = DapperProvider.GetConnection())
            {
                string sql = "UPDATE Users SET LastLogin = GETDATE() WHERE UserId = @Id";
                int rowsAffected = conn.Execute(sql, new { Id = userID });
                return rowsAffected > 0;
            }
        }

        /// <summary>
        /// Lấy toàn bộ danh sách người dùng
        /// </summary>
        public IEnumerable<UserDTO> GetAll()
        {
            string sql = "SELECT * FROM Users ORDER BY CreatedAt DESC";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<UserDTO>(sql);
        }

        /// <summary>
        /// Lấy users phân trang
        /// </summary>
        public IEnumerable<UserDTO> GetUsersPaged(int pageNumber, int pageSize)
        {
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
        /// Tìm kiếm người dùng
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
        /// Cập nhật trạng thái IsActive
        /// </summary>
        public bool UpdateStatus(int userId, bool isActive)
        {
            string sql = "UPDATE Users SET IsActive = @Status, UpdatedAt = GETDATE() WHERE UserId = @Id";
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