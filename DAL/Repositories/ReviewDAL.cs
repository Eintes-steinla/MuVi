using Dapper;
using Microsoft.Data.SqlClient;
using MuVi.DAL;
using MuVi.DTO.DTOs;

namespace Muvi.DAL
{
    public class ReviewDAL
    {
        /// <summary>
        /// Kiểm tra user đã đánh giá phim chưa
        /// </summary>
        public bool HasUserReviewedMovie(int userId, int movieId)
        {
            string sql = "SELECT COUNT(*) FROM Reviews WHERE UserID = @UserId AND MovieID = @MovieId";
            using SqlConnection conn = DapperProvider.GetConnection();
            int count = conn.ExecuteScalar<int>(sql, new { UserId = userId, MovieId = movieId });
            return count > 0;
        }

        /// <summary>
        /// Thêm đánh giá mới
        /// </summary>
        public bool AddReview(ReviewDTO review)
        {
            string sql = @"
            INSERT INTO Reviews
            (
                MovieID,
                UserID,
                Rating,
                Comment,
                LikeCount,
                CreatedAt,
                UpdatedAt
            )
            VALUES
            (
                @MovieID,
                @UserID,
                @Rating,
                @Comment,
                0,
                GETDATE(),
                GETDATE()
            )";

            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new
            {
                review.MovieID,
                review.UserID,
                review.Rating,
                review.Comment
            });

            return rows > 0;
        }

        /// <summary>
        /// Cập nhật đánh giá
        /// </summary>
        public bool UpdateReview(ReviewDTO review)
        {
            string sql = @"
            UPDATE Reviews 
            SET 
                Rating = @Rating,
                Comment = @Comment,
                UpdatedAt = GETDATE()
            WHERE ReviewID = @ReviewID";

            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new
            {
                review.ReviewID,
                review.Rating,
                review.Comment
            });

            return rows > 0;
        }

        /// <summary>
        /// Lấy đánh giá theo ID
        /// </summary>
        public ReviewDTO? GetById(int reviewId)
        {
            string sql = @"
            SELECT 
                r.*,
                m.Title AS MovieTitle,
                u.Username
            FROM Reviews r
            INNER JOIN Movies m ON r.MovieID = m.MovieID
            INNER JOIN Users u ON r.UserID = u.UserID
            WHERE r.ReviewID = @ReviewId";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.QueryFirstOrDefault<ReviewDTO>(sql, new { ReviewId = reviewId });
        }

        /// <summary>
        /// Lấy toàn bộ đánh giá
        /// </summary>
        public IEnumerable<ReviewDTO> GetAll()
        {
            string sql = @"
            SELECT 
                r.*,
                m.Title AS MovieTitle,
                u.Username
            FROM Reviews r
            INNER JOIN Movies m ON r.MovieID = m.MovieID
            INNER JOIN Users u ON r.UserID = u.UserID
            ORDER BY r.CreatedAt DESC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<ReviewDTO>(sql);
        }

        /// <summary>
        /// Lấy đánh giá theo phim
        /// </summary>
        public IEnumerable<ReviewDTO> GetByMovieId(int movieId)
        {
            string sql = @"
            SELECT 
                r.*,
                m.Title AS MovieTitle,
                u.Username
            FROM Reviews r
            INNER JOIN Movies m ON r.MovieID = m.MovieID
            INNER JOIN Users u ON r.UserID = u.UserID
            WHERE r.MovieID = @MovieId
            ORDER BY r.CreatedAt DESC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<ReviewDTO>(sql, new { MovieId = movieId });
        }

        /// <summary>
        /// Lấy đánh giá theo user
        /// </summary>
        public IEnumerable<ReviewDTO> GetByUserId(int userId)
        {
            string sql = @"
            SELECT 
                r.*,
                m.Title AS MovieTitle,
                u.Username
            FROM Reviews r
            INNER JOIN Movies m ON r.MovieID = m.MovieID
            INNER JOIN Users u ON r.UserID = u.UserID
            WHERE r.UserID = @UserId
            ORDER BY r.CreatedAt DESC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<ReviewDTO>(sql, new { UserId = userId });
        }

        /// <summary>
        /// Tìm kiếm đánh giá
        /// </summary>
        public IEnumerable<ReviewDTO> Search(string keyword)
        {
            string sql = @"
            SELECT 
                r.*,
                m.Title AS MovieTitle,
                u.Username
            FROM Reviews r
            INNER JOIN Movies m ON r.MovieID = m.MovieID
            INNER JOIN Users u ON r.UserID = u.UserID
            WHERE m.Title LIKE @Key 
            OR u.Username LIKE @Key 
            OR r.Comment LIKE @Key
            ORDER BY r.CreatedAt DESC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<ReviewDTO>(sql, new { Key = $"%{keyword}%" });
        }

        /// <summary>
        /// Xóa đánh giá
        /// </summary>
        public bool Delete(int reviewId)
        {
            string sql = "DELETE FROM Reviews WHERE ReviewID = @Id";
            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new { Id = reviewId });
            return rows > 0;
        }

        /// <summary>
        /// Cập nhật số lượt like
        /// </summary>
        public bool UpdateLikeCount(int reviewId, int increment)
        {
            string sql = @"
            UPDATE Reviews 
            SET LikeCount = ISNULL(LikeCount, 0) + @Increment 
            WHERE ReviewID = @ReviewId";

            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new { ReviewId = reviewId, Increment = increment });
            return rows > 0;
        }

        /// <summary>
        /// Lấy điểm đánh giá trung bình của phim
        /// </summary>
        public decimal GetAverageRating(int movieId)
        {
            string sql = "SELECT ISNULL(AVG(CAST(Rating AS DECIMAL(3,2))), 0) FROM Reviews WHERE MovieID = @MovieId";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.ExecuteScalar<decimal>(sql, new { MovieId = movieId });
        }

        /// <summary>
        /// Đếm số đánh giá của phim
        /// </summary>
        public int GetReviewCount(int movieId)
        {
            string sql = "SELECT COUNT(*) FROM Reviews WHERE MovieID = @MovieId";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.ExecuteScalar<int>(sql, new { MovieId = movieId });
        }

        /// <summary>
        /// Lấy top đánh giá được like nhiều nhất
        /// </summary>
        public IEnumerable<ReviewDTO> GetTopLikedReviews(int topCount)
        {
            string sql = @"
            SELECT TOP (@TopCount)
                r.*,
                m.Title AS MovieTitle,
                u.Username
            FROM Reviews r
            INNER JOIN Movies m ON r.MovieID = m.MovieID
            INNER JOIN Users u ON r.UserID = u.UserID
            ORDER BY r.LikeCount DESC, r.CreatedAt DESC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<ReviewDTO>(sql, new { TopCount = topCount });
        }
    }
}