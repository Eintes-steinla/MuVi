using Dapper;
using Microsoft.Data.SqlClient;
using MuVi.DAL;
using MuVi.DTO.DTOs;

namespace Muvi.DAL
{
    public class MovieDAL
    {
        /// <summary>
        /// Kiểm tra tiêu đề phim đã tồn tại chưa
        /// </summary>
        public bool IsTitleExists(string title)
        {
            string sql = "SELECT COUNT(*) FROM Movies WHERE Title = @Title";
            using SqlConnection conn = DapperProvider.GetConnection();
            int count = conn.ExecuteScalar<int>(sql, new { Title = title });
            return count > 0;
        }

        /// <summary>
        /// Kiểm tra tiêu đề phim đã tồn tại chưa (trừ movie hiện tại)
        /// </summary>
        public bool IsTitleExists(string title, int movieId)
        {
            string sql = "SELECT COUNT(*) FROM Movies WHERE Title = @Title AND MovieID != @MovieID";
            using SqlConnection conn = DapperProvider.GetConnection();
            int count = conn.ExecuteScalar<int>(sql, new { Title = title, MovieID = movieId });
            return count > 0;
        }

        /// <summary>
        /// Thêm phim mới
        /// </summary>
        public bool AddMovie(MovieDTO movie)
        {
            string sql = @"
            INSERT INTO Movies
            (
                Title,
                MovieType,
                CountryID,
                Director,
                ReleaseYear,
                [Description],
                PosterPath,
                TrailerURL,
                VideoPath,
                Duration,
                TotalEpisodes,
                Rating,
                ViewCount,
                [Status],
                CreatedAt,
                UpdatedAt
            )
            VALUES
            (
                @Title,
                @MovieType,
                @CountryID,
                @Director,
                @ReleaseYear,
                @Description,
                @PosterPath,
                @TrailerURL,
                @VideoPath,
                @Duration,
                @TotalEpisodes,
                @Rating,
                @ViewCount,
                @Status,
                GETDATE(),
                GETDATE()
            )";

            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new
            {
                movie.Title,
                movie.MovieType,
                movie.CountryID,
                movie.Director,
                movie.ReleaseYear,
                movie.Description,
                movie.PosterPath,
                movie.TrailerURL,
                movie.VideoPath,
                movie.Duration,
                movie.TotalEpisodes,
                movie.Rating,
                movie.ViewCount,
                movie.Status
            });

            return rows > 0;
        }

        /// <summary>
        /// Cập nhật thông tin phim
        /// </summary>
        public bool UpdateMovie(MovieDTO movie)
        {
            string sql = @"
            UPDATE Movies 
            SET 
                Title = @Title,
                MovieType = @MovieType,
                CountryID = @CountryID,
                Director = @Director,
                ReleaseYear = @ReleaseYear,
                [Description] = @Description,
                PosterPath = @PosterPath,
                TrailerURL = @TrailerURL,
                VideoPath = @VideoPath,
                Duration = @Duration,
                TotalEpisodes = @TotalEpisodes,
                Rating = @Rating,
                ViewCount = @ViewCount,
                [Status] = @Status,
                UpdatedAt = GETDATE()
            WHERE MovieID = @MovieID";

            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new
            {
                movie.MovieID,
                movie.Title,
                movie.MovieType,
                movie.CountryID,
                movie.Director,
                movie.ReleaseYear,
                movie.Description,
                movie.PosterPath,
                movie.TrailerURL,
                movie.VideoPath,
                movie.Duration,
                movie.TotalEpisodes,
                movie.Rating,
                movie.ViewCount,
                movie.Status
            });

            return rows > 0;
        }

        /// <summary>
        /// Lấy phim theo ID
        /// </summary>
        public MovieDTO? GetById(int movieId)
        {
            string sql = @"
            SELECT m.*, c.CountryName
            FROM Movies m
            LEFT JOIN Countries c ON m.CountryID = c.CountryID
            WHERE m.MovieID = @MovieId";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.QueryFirstOrDefault<MovieDTO>(sql, new { MovieId = movieId });
        }

        /// <summary>
        /// Lấy toàn bộ danh sách phim (kèm tên quốc gia)
        /// </summary>
        public IEnumerable<MovieDTO> GetAll()
        {
            string sql = @"
            SELECT m.*, c.CountryName
            FROM Movies m
            LEFT JOIN Countries c ON m.CountryID = c.CountryID
            ORDER BY m.CreatedAt DESC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<MovieDTO>(sql);
        }

        /// <summary>
        /// Lấy phim phân trang
        /// </summary>
        public IEnumerable<MovieDTO> GetMoviesPaged(int pageNumber, int pageSize)
        {
            string sql = @"
            SELECT m.*, c.CountryName
            FROM Movies m
            LEFT JOIN Countries c ON m.CountryID = c.CountryID
            ORDER BY m.MovieID ASC
            OFFSET @Offset ROWS 
            FETCH NEXT @PageSize ROWS ONLY";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<MovieDTO>(sql, new
            {
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize
            });
        }

        /// <summary>
        /// Tìm kiếm phim
        /// </summary>
        public IEnumerable<MovieDTO> SearchMovies(string keyword)
        {
            string sql = @"
            SELECT m.*, c.CountryName
            FROM Movies m
            LEFT JOIN Countries c ON m.CountryID = c.CountryID
            WHERE m.Title LIKE @Key OR m.Director LIKE @Key OR c.CountryName LIKE @Key
            ORDER BY m.CreatedAt DESC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<MovieDTO>(sql, new { Key = $"%{keyword}%" });
        }

        /// <summary>
        /// Xóa phim
        /// </summary>
        public bool Delete(int movieId)
        {
            string sql = "DELETE FROM Movies WHERE MovieID = @Id";
            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new { Id = movieId });
            return rows > 0;
        }

        /// <summary>
        /// Cập nhật lượt xem
        /// </summary>
        public bool UpdateViewCount(int movieId)
        {
            string sql = "UPDATE Movies SET ViewCount = ISNULL(ViewCount, 0) + 1, UpdatedAt = GETDATE() WHERE MovieID = @Id";
            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new { Id = movieId });
            return rows > 0;
        }
    }
}