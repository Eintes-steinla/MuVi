using Dapper;
using Microsoft.Data.SqlClient;
using MuVi.DAL;
using MuVi.DTO.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Muvi.DAL
{
    public class ViewHistoryDAL
    {
        /// <summary>
        /// Lấy lịch sử xem của một User (kèm thông tin phim)
        /// </summary>
        public List<ViewHistoryDTO> GetByUserId(int userId)
        {
            string sql = @"
                SELECT v.*, m.Title AS MovieTitle, m.PosterPath, e.EpisodeNumber
                FROM ViewHistory v
                JOIN Movies m ON v.MovieID = m.MovieID
                LEFT JOIN Episodes e ON v.EpisodeID = e.EpisodeID
                WHERE v.UserID = @UserId
                ORDER BY v.WatchedAt DESC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<ViewHistoryDTO>(sql, new { UserId = userId }).ToList();
        }

        /// <summary>
        /// Lưu hoặc cập nhật lịch sử xem
        /// </summary>
        public bool SaveOrUpdateHistory(ViewHistoryDTO history)
        {
            // Kiểm tra xem user này đã từng xem phim/tập này chưa
            string checkSql = @"SELECT HistoryID FROM ViewHistory 
                                WHERE UserID = @UserID AND MovieID = @MovieID 
                                AND (EpisodeID = @EpisodeID OR (EpisodeID IS NULL AND @EpisodeID IS NULL))";

            using SqlConnection conn = DapperProvider.GetConnection();
            var existingId = conn.ExecuteScalar<int?>(checkSql, history);

            if (existingId.HasValue)
            {
                // Nếu đã có, tiến hành UPDATE
                string updateSql = @"
                    UPDATE ViewHistory 
                    SET WatchedAt = GETDATE(), 
                        WatchDuration = @WatchDuration, 
                        IsCompleted = @IsCompleted
                    WHERE HistoryID = @Id";
                return conn.Execute(updateSql, new { Id = existingId.Value, history.WatchDuration, history.IsCompleted }) > 0;
            }
            else
            {
                // Nếu chưa có, tiến hành INSERT
                string insertSql = @"
                    INSERT INTO ViewHistory (UserID, MovieID, EpisodeID, WatchedAt, WatchDuration, IsCompleted)
                    VALUES (@UserID, @MovieID, @EpisodeID, GETDATE(), @WatchDuration, @IsCompleted)";
                return conn.Execute(insertSql, history) > 0;
            }
        }

        public bool DeleteHistory(int historyId)
        {
            string sql = "DELETE FROM ViewHistory WHERE HistoryID = @Id";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Execute(sql, new { Id = historyId }) > 0;
        }
    }
}