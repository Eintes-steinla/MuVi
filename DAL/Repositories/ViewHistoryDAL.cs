using Dapper;
using Microsoft.Data.SqlClient;
using MuVi.DAL;
using MuVi.DTO.DTOs;

namespace Muvi.DAL
{
    public class ViewHistoryDAL
    {
        /// <summary>
        /// Thêm hoặc cập nhật lịch sử xem
        /// </summary>
        public bool AddOrUpdateHistory(ViewHistoryDTO history)
        {
            string sql = @"
            IF EXISTS (SELECT 1 FROM ViewHistory 
                       WHERE UserID = @UserID 
                       AND MovieID = @MovieID 
                       AND (@EpisodeID IS NULL OR EpisodeID = @EpisodeID))
            BEGIN
                UPDATE ViewHistory
                SET WatchedAt = GETDATE(),
                    WatchDuration = @WatchDuration,
                    IsCompleted = @IsCompleted
                WHERE UserID = @UserID 
                AND MovieID = @MovieID 
                AND (@EpisodeID IS NULL OR EpisodeID = @EpisodeID)
            END
            ELSE
            BEGIN
                INSERT INTO ViewHistory 
                (UserID, MovieID, EpisodeID, WatchedAt, WatchDuration, IsCompleted)
                VALUES 
                (@UserID, @MovieID, @EpisodeID, GETDATE(), @WatchDuration, @IsCompleted)
            END";

            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new
            {
                history.UserID,
                history.MovieID,
                history.EpisodeID,
                history.WatchDuration,
                history.IsCompleted
            });

            return rows > 0;
        }

        /// <summary>
        /// Lấy lịch sử xem theo ID
        /// </summary>
        public ViewHistoryDTO? GetById(int historyId)
        {
            string sql = @"
            SELECT 
                vh.*,
                u.Username,
                m.Title AS MovieTitle,
                m.MovieType,
                e.EpisodeNumber,
                e.Title AS EpisodeTitle
            FROM ViewHistory vh
            INNER JOIN Users u ON vh.UserID = u.UserID
            INNER JOIN Movies m ON vh.MovieID = m.MovieID
            LEFT JOIN Episodes e ON vh.EpisodeID = e.EpisodeID
            WHERE vh.HistoryID = @HistoryId";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.QueryFirstOrDefault<ViewHistoryDTO>(sql, new { HistoryId = historyId });
        }

        /// <summary>
        /// Lấy toàn bộ lịch sử xem
        /// </summary>
        public IEnumerable<ViewHistoryDTO> GetAll()
        {
            string sql = @"
            SELECT 
                vh.*,
                u.Username,
                m.Title AS MovieTitle,
                m.MovieType,
                e.EpisodeNumber,
                e.Title AS EpisodeTitle
            FROM ViewHistory vh
            INNER JOIN Users u ON vh.UserID = u.UserID
            INNER JOIN Movies m ON vh.MovieID = m.MovieID
            LEFT JOIN Episodes e ON vh.EpisodeID = e.EpisodeID
            ORDER BY vh.WatchedAt DESC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<ViewHistoryDTO>(sql);
        }

        /// <summary>
        /// Lấy lịch sử xem của một user
        /// </summary>
        public IEnumerable<ViewHistoryDTO> GetByUserId(int userId)
        {
            string sql = @"
            SELECT 
                vh.*,
                u.Username,
                m.Title AS MovieTitle,
                m.MovieType,
                e.EpisodeNumber,
                e.Title AS EpisodeTitle
            FROM ViewHistory vh
            INNER JOIN Users u ON vh.UserID = u.UserID
            INNER JOIN Movies m ON vh.MovieID = m.MovieID
            LEFT JOIN Episodes e ON vh.EpisodeID = e.EpisodeID
            WHERE vh.UserID = @UserId
            ORDER BY vh.WatchedAt DESC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<ViewHistoryDTO>(sql, new { UserId = userId });
        }

        /// <summary>
        /// Lấy lịch sử xem của một phim
        /// </summary>
        public IEnumerable<ViewHistoryDTO> GetByMovieId(int movieId)
        {
            string sql = @"
            SELECT 
                vh.*,
                u.Username,
                m.Title AS MovieTitle,
                m.MovieType,
                e.EpisodeNumber,
                e.Title AS EpisodeTitle
            FROM ViewHistory vh
            INNER JOIN Users u ON vh.UserID = u.UserID
            INNER JOIN Movies m ON vh.MovieID = m.MovieID
            LEFT JOIN Episodes e ON vh.EpisodeID = e.EpisodeID
            WHERE vh.MovieID = @MovieId
            ORDER BY vh.WatchedAt DESC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<ViewHistoryDTO>(sql, new { MovieId = movieId });
        }

        /// <summary>
        /// Tìm kiếm lịch sử xem
        /// </summary>
        public IEnumerable<ViewHistoryDTO> Search(string keyword)
        {
            string sql = @"
            SELECT 
                vh.*,
                u.Username,
                m.Title AS MovieTitle,
                m.MovieType,
                e.EpisodeNumber,
                e.Title AS EpisodeTitle
            FROM ViewHistory vh
            INNER JOIN Users u ON vh.UserID = u.UserID
            INNER JOIN Movies m ON vh.MovieID = m.MovieID
            LEFT JOIN Episodes e ON vh.EpisodeID = e.EpisodeID
            WHERE u.Username LIKE @Key 
            OR m.Title LIKE @Key 
            OR e.Title LIKE @Key
            ORDER BY vh.WatchedAt DESC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<ViewHistoryDTO>(sql, new { Key = $"%{keyword}%" });
        }

        /// <summary>
        /// Xóa lịch sử xem
        /// </summary>
        public bool Delete(int historyId)
        {
            string sql = "DELETE FROM ViewHistory WHERE HistoryID = @Id";
            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new { Id = historyId });
            return rows > 0;
        }

        /// <summary>
        /// Xóa toàn bộ lịch sử của user
        /// </summary>
        public bool DeleteByUserId(int userId)
        {
            string sql = "DELETE FROM ViewHistory WHERE UserID = @UserId";
            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new { UserId = userId });
            return rows > 0;
        }

        /// <summary>
        /// Xóa toàn bộ lịch sử của phim
        /// </summary>
        public bool DeleteByMovieId(int movieId)
        {
            string sql = "DELETE FROM ViewHistory WHERE MovieID = @MovieId";
            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new { MovieId = movieId });
            return rows > 0;
        }

        /// <summary>
        /// Đếm tổng số lịch sử
        /// </summary>
        public int GetTotalCount()
        {
            string sql = "SELECT COUNT(*) FROM ViewHistory";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.ExecuteScalar<int>(sql);
        }

        /// <summary>
        /// Đếm lịch sử theo user
        /// </summary>
        public int GetCountByUserId(int userId)
        {
            string sql = "SELECT COUNT(*) FROM ViewHistory WHERE UserID = @UserId";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.ExecuteScalar<int>(sql, new { UserId = userId });
        }
    }
}