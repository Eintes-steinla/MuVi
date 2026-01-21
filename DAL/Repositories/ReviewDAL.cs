using Dapper;
using Microsoft.Data.SqlClient;
using MuVi.DAL;
using MuVi.DTO.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Muvi.DAL
{
    public class ReviewDAL
    {
        /// <summary>
        /// Lấy danh sách bình luận của một bộ phim
        /// </summary>
        public List<ReviewDTO> GetByMovieId(int movieId)
        {
            string sql = @"
                SELECT r.*, u.Username, u.Avatar
                FROM Reviews r
                JOIN Users u ON r.UserID = u.UserID
                WHERE r.MovieID = @MovieId
                ORDER BY r.CreatedAt DESC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<ReviewDTO>(sql, new { MovieId = movieId }).ToList();
        }

        public bool Insert(ReviewDTO review)
        {
            string sql = @"
                INSERT INTO Reviews (MovieID, UserID, Rating, Comment, LikeCount, CreatedAt, UpdatedAt)
                VALUES (@MovieID, @UserID, @Rating, @Comment, 0, GETDATE(), GETDATE())";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Execute(sql, review) > 0;
        }

        public bool Update(ReviewDTO review)
        {
            string sql = @"
                UPDATE Reviews 
                SET Rating = @Rating, Comment = @Comment, UpdatedAt = GETDATE()
                WHERE ReviewID = @ReviewID";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Execute(sql, review) > 0;
        }

        public bool Delete(int reviewId)
        {
            string sql = "DELETE FROM Reviews WHERE ReviewID = @Id";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Execute(sql, new { Id = reviewId }) > 0;
        }

        /// <summary>
        /// Kiểm tra xem user đã review phim này chưa
        /// </summary>
        public bool HasUserReviewed(int userId, int movieId)
        {
            string sql = "SELECT COUNT(*) FROM Reviews WHERE UserID = @UserId AND MovieID = @MovieId";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.ExecuteScalar<int>(sql, new { UserId = userId, MovieId = movieId }) > 0;
        }
    }
}