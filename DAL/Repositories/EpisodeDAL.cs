using Dapper;
using Microsoft.Data.SqlClient;
using MuVi.DAL;
using MuVi.DTO.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Muvi.DAL
{
    public class EpisodeDAL
    {
        /// <summary>
        /// Lấy tất cả tập phim của một bộ phim cụ thể
        /// </summary>
        public List<EpisodeDTO> GetByMovieId(int movieId)
        {
            string sql = "SELECT * FROM Episodes WHERE MovieID = @MovieId ORDER BY EpisodeNumber ASC";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<EpisodeDTO>(sql, new { MovieId = movieId }).ToList();
        }

        public EpisodeDTO? GetById(int id)
        {
            string sql = "SELECT * FROM Episodes WHERE EpisodeID = @Id";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.QueryFirstOrDefault<EpisodeDTO>(sql, new { Id = id });
        }

        public bool Insert(EpisodeDTO episode)
        {
            string sql = @"
                INSERT INTO Episodes (MovieID, EpisodeNumber, Title, Description, Duration, VideoPath, ReleaseDate, ViewCount, CreatedAt)
                VALUES (@MovieID, @EpisodeNumber, @Title, @Description, @Duration, @VideoPath, @ReleaseDate, 0, GETDATE())";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Execute(sql, episode) > 0;
        }

        public bool Update(EpisodeDTO episode)
        {
            string sql = @"
                UPDATE Episodes 
                SET EpisodeNumber = @EpisodeNumber, 
                    Title = @Title, 
                    Description = @Description, 
                    Duration = @Duration, 
                    VideoPath = @VideoPath, 
                    ReleaseDate = @ReleaseDate
                WHERE EpisodeID = @EpisodeID";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Execute(sql, episode) > 0;
        }

        public bool Delete(int id)
        {
            string sql = "DELETE FROM Episodes WHERE EpisodeID = @Id";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Execute(sql, new { Id = id }) > 0;
        }

        /// <summary>
        /// Tăng lượt xem cho tập phim
        /// </summary>
        public bool IncrementViewCount(int episodeId)
        {
            string sql = "UPDATE Episodes SET ViewCount = ViewCount + 1 WHERE EpisodeID = @Id";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Execute(sql, new { Id = episodeId }) > 0;
        }
    }
}