using Dapper;
using Microsoft.Data.SqlClient;
using MuVi.DAL;
using MuVi.DTO.DTOs;

namespace Muvi.DAL
{
    public class EpisodeDAL
    {
        /// <summary>
        /// Kiểm tra số tập đã tồn tại trong phim chưa
        /// </summary>
        public bool IsEpisodeNumberExists(int movieId, int episodeNumber)
        {
            string sql = "SELECT COUNT(*) FROM Episodes WHERE MovieID = @MovieID AND EpisodeNumber = @EpisodeNumber";
            using SqlConnection conn = DapperProvider.GetConnection();
            int count = conn.ExecuteScalar<int>(sql, new { MovieID = movieId, EpisodeNumber = episodeNumber });
            return count > 0;
        }

        /// <summary>
        /// Kiểm tra số tập đã tồn tại chưa (trừ episode hiện tại)
        /// </summary>
        public bool IsEpisodeNumberExists(int movieId, int episodeNumber, int episodeId)
        {
            string sql = "SELECT COUNT(*) FROM Episodes WHERE MovieID = @MovieID AND EpisodeNumber = @EpisodeNumber AND EpisodeID != @EpisodeID";
            using SqlConnection conn = DapperProvider.GetConnection();
            int count = conn.ExecuteScalar<int>(sql, new { MovieID = movieId, EpisodeNumber = episodeNumber, EpisodeID = episodeId });
            return count > 0;
        }

        /// <summary>
        /// Thêm tập phim mới
        /// </summary>
        public bool AddEpisode(EpisodeDTO episode)
        {
            string sql = @"
            INSERT INTO Episodes
            (
                MovieID,
                EpisodeNumber,
                Title,
                [Description],
                Duration,
                PosterPath,
                VideoPath,
                ReleaseDate,
                ViewCount,
                CreatedAt
            )
            VALUES
            (
                @MovieID,
                @EpisodeNumber,
                @Title,
                @Description,
                @Duration,
                @PosterPath,
                @VideoPath,
                @ReleaseDate,
                @ViewCount,
                GETDATE()
            )";

            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new
            {
                episode.MovieID,
                episode.EpisodeNumber,
                episode.Title,
                episode.Description,
                episode.Duration,
                episode.PosterPath,
                episode.VideoPath,
                episode.ReleaseDate,
                episode.ViewCount
            });

            return rows > 0;
        }

        /// <summary>
        /// Cập nhật thông tin tập phim
        /// </summary>
        public bool UpdateEpisode(EpisodeDTO episode)
        {
            string sql = @"
            UPDATE Episodes 
            SET 
                MovieID = @MovieID,
                EpisodeNumber = @EpisodeNumber,
                Title = @Title,
                [Description] = @Description,
                Duration = @Duration,
                PosterPath = @PosterPath,
                VideoPath = @VideoPath,
                ReleaseDate = @ReleaseDate,
                ViewCount = @ViewCount
            WHERE EpisodeID = @EpisodeID";

            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new
            {
                episode.EpisodeID,
                episode.MovieID,
                episode.EpisodeNumber,
                episode.Title,
                episode.Description,
                episode.Duration,
                episode.PosterPath,
                episode.VideoPath,
                episode.ReleaseDate,
                episode.ViewCount
            });

            return rows > 0;
        }

        /// <summary>
        /// Lấy tập phim theo ID
        /// </summary>
        public EpisodeDTO? GetById(int episodeId)
        {
            string sql = @"
            SELECT e.*, m.Title as MovieTitle, m.MovieType
            FROM Episodes e
            LEFT JOIN Movies m ON e.MovieID = m.MovieID
            WHERE e.EpisodeID = @EpisodeId";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.QueryFirstOrDefault<EpisodeDTO>(sql, new { EpisodeId = episodeId });
        }

        /// <summary>
        /// Lấy toàn bộ danh sách tập phim (kèm tên phim)
        /// </summary>
        public IEnumerable<EpisodeDTO> GetAll()
        {
            string sql = @"
            SELECT e.*, m.Title as MovieTitle, m.MovieType
            FROM Episodes e
            LEFT JOIN Movies m ON e.MovieID = m.MovieID
            ORDER BY e.CreatedAt DESC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<EpisodeDTO>(sql);
        }

        /// <summary>
        /// Lấy tập phim theo MovieID
        /// </summary>
        public IEnumerable<EpisodeDTO> GetByMovieId(int movieId)
        {
            string sql = @"
            SELECT e.*, m.Title as MovieTitle, m.MovieType
            FROM Episodes e
            LEFT JOIN Movies m ON e.MovieID = m.MovieID
            WHERE e.MovieID = @MovieID
            ORDER BY e.EpisodeNumber ASC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<EpisodeDTO>(sql, new { MovieID = movieId });
        }

        /// <summary>
        /// Tìm kiếm tập phim
        /// </summary>
        public IEnumerable<EpisodeDTO> SearchEpisodes(string keyword)
        {
            string sql = @"
            SELECT e.*, m.Title as MovieTitle, m.MovieType
            FROM Episodes e
            LEFT JOIN Movies m ON e.MovieID = m.MovieID
            WHERE e.Title LIKE @Key OR m.Title LIKE @Key OR e.Description LIKE @Key
            ORDER BY e.CreatedAt DESC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<EpisodeDTO>(sql, new { Key = $"%{keyword}%" });
        }

        /// <summary>
        /// Xóa tập phim
        /// </summary>
        public bool Delete(int episodeId)
        {
            string sql = "DELETE FROM Episodes WHERE EpisodeID = @Id";
            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new { Id = episodeId });
            return rows > 0;
        }

        /// <summary>
        /// Cập nhật lượt xem
        /// </summary>
        public bool UpdateViewCount(int episodeId)
        {
            string sql = "UPDATE Episodes SET ViewCount = ISNULL(ViewCount, 0) + 1 WHERE EpisodeID = @Id";
            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new { Id = episodeId });
            return rows > 0;
        }
    }
}