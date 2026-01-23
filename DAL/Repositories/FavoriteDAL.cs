using Dapper;
using Microsoft.Data.SqlClient;
using MuVi.DAL;
using MuVi.DTO.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Muvi.DAL
{
    public class FavoriteDAL
    {
        // Lấy danh sách phim yêu thích kèm thông tin chi tiết phim
        public List<FavoriteDTO> GetFavoritesByUserId(int userId)
        {
            string sql = @"
                SELECT f.UserID, f.MovieID, f.CreatedAt as AddedAt, 
                       m.Title, m.PosterPath, m.ReleaseYear
                FROM Favorites f
                JOIN Movies m ON f.MovieID = m.MovieID
                WHERE f.UserID = @UserId
                ORDER BY f.CreatedAt DESC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<FavoriteDTO>(sql, new { UserId = userId }).ToList();
        }

        // Thêm một bộ phim vào danh sách yêu thích
        public bool Insert(int userId, int movieId)
        {
            string sql = "INSERT INTO Favorites (UserID, MovieID, CreatedAt) VALUES (@UserId, @MovieId, GETDATE())";
            using SqlConnection conn = DapperProvider.GetConnection();
            try
            {
                return conn.Execute(sql, new { UserId = userId, MovieId = movieId }) > 0;
            }
            catch
            {
                return false; // Tránh crash nếu đã tồn tại (do Primary Key/Unique)
            }
        }

        // Xóa khỏi danh sách yêu thích
        public bool Delete(int userId, int movieId)
        {
            string sql = "DELETE FROM Favorites WHERE UserID = @UserId AND MovieID = @MovieId";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Execute(sql, new { UserId = userId, MovieId = movieId }) > 0;
        }

        // Kiểm tra xem user đã thích phim này chưa
        public bool IsFavorite(int userId, int movieId)
        {
            string sql = "SELECT COUNT(1) FROM Favorites WHERE UserID = @UserId AND MovieID = @MovieId";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.ExecuteScalar<int>(sql, new { UserId = userId, MovieId = movieId }) > 0;
        }
    }
}