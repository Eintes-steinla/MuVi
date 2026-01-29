using Dapper;
using Microsoft.Data.SqlClient;
using MuVi.DAL;
using MuVi.DTO.DTOs;
using System;
using System.Collections.Generic;

namespace Muvi.DAL
{
    public class FavoriteDAL
    {
        /// <summary>
        /// Kiểm tra phim đã được yêu thích bởi user chưa
        /// </summary>
        public bool IsFavoriteExists(int userId, int movieId)
        {
            string sql = "SELECT COUNT(*) FROM Favorites WHERE UserID = @UserID AND MovieID = @MovieID";
            using SqlConnection conn = DapperProvider.GetConnection();
            int count = conn.ExecuteScalar<int>(sql, new { UserID = userId, MovieID = movieId });
            return count > 0;
        }

        /// <summary>
        /// Thêm phim vào danh sách yêu thích
        /// </summary>
        public bool AddFavorite(int userId, int movieId)
        {
            string sql = @"
            INSERT INTO Favorites
            (
                UserID,
                MovieID,
                AddedAt
            )
            VALUES
            (
                @UserID,
                @MovieID,
                GETDATE()
            )";

            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new
            {
                UserID = userId,
                MovieID = movieId
            });

            return rows > 0;
        }

        /// <summary>
        /// Xóa phim khỏi danh sách yêu thích
        /// </summary>
        public bool DeleteFavorite(int userId, int movieId)
        {
            string sql = "DELETE FROM Favorites WHERE UserID = @UserID AND MovieID = @MovieID";
            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new { UserID = userId, MovieID = movieId });
            return rows > 0;
        }

        /// <summary>
        /// Lấy danh sách yêu thích theo UserID
        /// </summary>
        public IEnumerable<FavoriteDTO> GetFavoritesByUserId(int userId)
        {
            string sql = @"
            SELECT 
                f.UserID,
                f.MovieID,
                f.AddedAt,
                u.Username,
                m.Title AS MovieTitle,
                m.MovieType,
                m.ReleaseYear,
                m.PosterPath,
                m.Rating,
                STUFF((
                    SELECT ', ' + g.GenreName
                    FROM MovieCategory mc
                    INNER JOIN Genres g ON mc.GenreID = g.GenreID
                    WHERE mc.MovieID = m.MovieID
                    ORDER BY g.GenreName
                    FOR XML PATH('')
                ), 1, 2, '') AS GenreNames
            FROM Favorites f
            INNER JOIN Users u ON f.UserID = u.UserID
            INNER JOIN Movies m ON f.MovieID = m.MovieID
            WHERE f.UserID = @UserID
            ORDER BY f.AddedAt DESC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<FavoriteDTO>(sql, new { UserID = userId });
        }

        /// <summary>
        /// Lấy danh sách yêu thích theo MovieID
        /// </summary>
        public IEnumerable<FavoriteDTO> GetFavoritesByMovieId(int movieId)
        {
            string sql = @"
            SELECT 
                f.UserID,
                f.MovieID,
                f.AddedAt,
                u.Username,
                m.Title AS MovieTitle,
                m.MovieType,
                m.ReleaseYear,
                m.PosterPath,
                m.Rating
            FROM Favorites f
            INNER JOIN Users u ON f.UserID = u.UserID
            INNER JOIN Movies m ON f.MovieID = m.MovieID
            WHERE f.MovieID = @MovieID
            ORDER BY f.AddedAt DESC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<FavoriteDTO>(sql, new { MovieID = movieId });
        }

        /// <summary>
        /// Lấy toàn bộ danh sách yêu thích (kèm thông tin user và phim)
        /// </summary>
        public IEnumerable<FavoriteDTO> GetAll()
        {
            string sql = @"
            SELECT 
                f.UserID,
                f.MovieID,
                f.AddedAt,
                u.Username,
                m.Title AS MovieTitle,
                m.MovieType,
                m.ReleaseYear,
                m.PosterPath,
                m.Rating,
                STUFF((
                    SELECT ', ' + g.GenreName
                    FROM MovieCategory mc
                    INNER JOIN Genres g ON mc.GenreID = g.GenreID
                    WHERE mc.MovieID = m.MovieID
                    ORDER BY g.GenreName
                    FOR XML PATH('')
                ), 1, 2, '') AS GenreNames
            FROM Favorites f
            INNER JOIN Users u ON f.UserID = u.UserID
            INNER JOIN Movies m ON f.MovieID = m.MovieID
            ORDER BY f.AddedAt DESC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<FavoriteDTO>(sql);
        }

        /// <summary>
        /// Lấy yêu thích phân trang
        /// </summary>
        public IEnumerable<FavoriteDTO> GetFavoritesPaged(int pageNumber, int pageSize)
        {
            string sql = @"
            SELECT 
                f.UserID,
                f.MovieID,
                f.AddedAt,
                u.Username,
                m.Title AS MovieTitle,
                m.MovieType,
                m.ReleaseYear,
                m.PosterPath,
                m.Rating,
                STUFF((
                    SELECT ', ' + g.GenreName
                    FROM MovieCategory mc
                    INNER JOIN Genres g ON mc.GenreID = g.GenreID
                    WHERE mc.MovieID = m.MovieID
                    ORDER BY g.GenreName
                    FOR XML PATH('')
                ), 1, 2, '') AS GenreNames
            FROM Favorites f
            INNER JOIN Users u ON f.UserID = u.UserID
            INNER JOIN Movies m ON f.MovieID = m.MovieID
            ORDER BY f.AddedAt DESC
            OFFSET @Offset ROWS 
            FETCH NEXT @PageSize ROWS ONLY";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<FavoriteDTO>(sql, new
            {
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize
            });
        }

        /// <summary>
        /// Tìm kiếm yêu thích theo username hoặc tên phim
        /// </summary>
        public IEnumerable<FavoriteDTO> SearchFavorites(string keyword)
        {
            string sql = @"
            SELECT 
                f.UserID,
                f.MovieID,
                f.AddedAt,
                u.Username,
                m.Title AS MovieTitle,
                m.MovieType,
                m.ReleaseYear,
                m.PosterPath,
                m.Rating,
                STUFF((
                    SELECT ', ' + g.GenreName
                    FROM MovieCategory mc
                    INNER JOIN Genres g ON mc.GenreID = g.GenreID
                    WHERE mc.MovieID = m.MovieID
                    ORDER BY g.GenreName
                    FOR XML PATH('')
                ), 1, 2, '') AS GenreNames
            FROM Favorites f
            INNER JOIN Users u ON f.UserID = u.UserID
            INNER JOIN Movies m ON f.MovieID = m.MovieID
            WHERE u.Username LIKE @Key OR m.Title LIKE @Key
            ORDER BY f.AddedAt DESC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<FavoriteDTO>(sql, new { Key = $"%{keyword}%" });
        }

        /// <summary>
        /// Đếm số lượng yêu thích của một phim
        /// </summary>
        public int CountFavoritesByMovieId(int movieId)
        {
            string sql = "SELECT COUNT(*) FROM Favorites WHERE MovieID = @MovieID";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.ExecuteScalar<int>(sql, new { MovieID = movieId });
        }

        /// <summary>
        /// Đếm số lượng yêu thích của một user
        /// </summary>
        public int CountFavoritesByUserId(int userId)
        {
            string sql = "SELECT COUNT(*) FROM Favorites WHERE UserID = @UserID";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.ExecuteScalar<int>(sql, new { UserID = userId });
        }

        /// <summary>
        /// Xóa tất cả yêu thích của một user
        /// </summary>
        public bool DeleteAllFavoritesByUserId(int userId)
        {
            string sql = "DELETE FROM Favorites WHERE UserID = @UserID";
            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new { UserID = userId });
            return rows > 0;
        }

        /// <summary>
        /// Xóa tất cả yêu thích của một phim
        /// </summary>
        public bool DeleteAllFavoritesByMovieId(int movieId)
        {
            string sql = "DELETE FROM Favorites WHERE MovieID = @MovieID";
            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new { MovieID = movieId });
            return rows > 0;
        }
    }
}