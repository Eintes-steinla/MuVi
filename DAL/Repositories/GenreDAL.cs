using Dapper;
using Microsoft.Data.SqlClient;
using MuVi.DAL;
using MuVi.DTO.DTOs;

namespace Muvi.DAL
{
    public class GenreDAL
    {
        /// <summary>
        /// Kiểm tra tên thể loại đã tồn tại chưa
        /// </summary>
        public bool IsGenreNameExists(string genreName)
        {
            string sql = "SELECT COUNT(*) FROM Genres WHERE GenreName = @GenreName";
            using SqlConnection conn = DapperProvider.GetConnection();
            int count = conn.ExecuteScalar<int>(sql, new { GenreName = genreName });
            return count > 0;
        }

        /// <summary>
        /// Kiểm tra tên thể loại đã tồn tại chưa (trừ genre hiện tại)
        /// </summary>
        public bool IsGenreNameExists(string genreName, int genreId)
        {
            string sql = "SELECT COUNT(*) FROM Genres WHERE GenreName = @GenreName AND GenreID != @GenreID";
            using SqlConnection conn = DapperProvider.GetConnection();
            int count = conn.ExecuteScalar<int>(sql, new { GenreName = genreName, GenreID = genreId });
            return count > 0;
        }

        /// <summary>
        /// Thêm thể loại mới
        /// </summary>
        public bool AddGenre(GenreDTO genre)
        {
            string sql = @"
            INSERT INTO Genres
            (
                GenreName,
                [Description]
            )
            VALUES
            (
                @GenreName,
                @Description
            )";

            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new
            {
                genre.GenreName,
                genre.Description
            });

            return rows > 0;
        }

        /// <summary>
        /// Cập nhật thông tin thể loại
        /// </summary>
        public bool UpdateGenre(GenreDTO genre)
        {
            string sql = @"
            UPDATE Genres 
            SET 
                GenreName = @GenreName,
                [Description] = @Description
            WHERE GenreID = @GenreID";

            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new
            {
                genre.GenreID,
                genre.GenreName,
                genre.Description
            });

            return rows > 0;
        }

        /// <summary>
        /// Lấy thể loại theo ID
        /// </summary>
        public GenreDTO? GetById(int genreId)
        {
            string sql = @"
            SELECT * FROM Genres
            WHERE GenreID = @GenreId";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.QueryFirstOrDefault<GenreDTO>(sql, new { GenreId = genreId });
        }

        /// <summary>
        /// Lấy toàn bộ danh sách thể loại
        /// </summary>
        public IEnumerable<GenreDTO> GetAll()
        {
            string sql = @"
            SELECT * FROM Genres
            ORDER BY GenreName ASC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<GenreDTO>(sql);
        }

        /// <summary>
        /// Lấy thể loại phân trang
        /// </summary>
        public IEnumerable<GenreDTO> GetGenresPaged(int pageNumber, int pageSize)
        {
            string sql = @"
            SELECT * FROM Genres
            ORDER BY GenreID ASC
            OFFSET @Offset ROWS 
            FETCH NEXT @PageSize ROWS ONLY";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<GenreDTO>(sql, new
            {
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize
            });
        }

        /// <summary>
        /// Tìm kiếm thể loại
        /// </summary>
        public IEnumerable<GenreDTO> SearchGenres(string keyword)
        {
            string sql = @"
            SELECT * FROM Genres
            WHERE GenreName LIKE @Key OR [Description] LIKE @Key
            ORDER BY GenreName ASC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<GenreDTO>(sql, new { Key = $"%{keyword}%" });
        }

        /// <summary>
        /// Xóa thể loại
        /// </summary>
        public bool Delete(int genreId)
        {
            string sql = "DELETE FROM Genres WHERE GenreID = @Id";
            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new { Id = genreId });
            return rows > 0;
        }

        // ============================================================
        // PHẦN THÊM MỚI - QUẢN LÝ MOVIE CATEGORY (QUAN HỆ PHIM - THỂ LOẠI)
        // ============================================================

        /// <summary>
        /// Lấy danh sách thể loại của một phim
        /// </summary>
        public IEnumerable<GenreDTO> GetGenresByMovieId(int movieId)
        {
            string sql = @"
            SELECT g.* 
            FROM Genres g
            INNER JOIN MovieCategory mc ON g.GenreID = mc.GenreID
            WHERE mc.MovieID = @MovieId
            ORDER BY g.GenreName ASC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<GenreDTO>(sql, new { MovieId = movieId });
        }

        /// <summary>
        /// Thêm thể loại cho phim (tránh trùng lặp)
        /// </summary>
        public bool AddMovieGenre(int movieId, int genreId)
        {
            string sql = @"
            IF NOT EXISTS (SELECT 1 FROM MovieCategory WHERE MovieID = @MovieId AND GenreID = @GenreId)
            BEGIN
                INSERT INTO MovieCategory (MovieID, GenreID)
                VALUES (@MovieId, @GenreId)
            END";

            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new { MovieId = movieId, GenreId = genreId });
            return rows >= 0; // Trả về true ngay cả khi đã tồn tại
        }

        /// <summary>
        /// Xóa một thể loại cụ thể của phim
        /// </summary>
        public bool DeleteMovieGenre(int movieId, int genreId)
        {
            string sql = "DELETE FROM MovieCategory WHERE MovieID = @MovieId AND GenreID = @GenreId";
            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new { MovieId = movieId, GenreId = genreId });
            return rows > 0;
        }

        /// <summary>
        /// Xóa tất cả thể loại của phim
        /// </summary>
        public bool DeleteMovieGenres(int movieId)
        {
            string sql = "DELETE FROM MovieCategory WHERE MovieID = @MovieId";
            using SqlConnection conn = DapperProvider.GetConnection();
            conn.Execute(sql, new { MovieId = movieId });
            return true; // Luôn trả về true (kể cả khi không có gì để xóa)
        }

        /// <summary>
        /// Thêm nhiều thể loại cho phim (bulk insert)
        /// </summary>
        public bool AddMovieGenres(int movieId, List<int> genreIds)
        {
            if (genreIds == null || !genreIds.Any())
                return true;

            using SqlConnection conn = DapperProvider.GetConnection();

            foreach (int genreId in genreIds)
            {
                AddMovieGenre(movieId, genreId);
            }

            return true;
        }

        /// <summary>
        /// Cập nhật thể loại của phim (xóa hết rồi thêm mới)
        /// </summary>
        public bool UpdateMovieGenres(int movieId, List<int> genreIds)
        {
            // Xóa tất cả thể loại cũ
            DeleteMovieGenres(movieId);

            // Thêm thể loại mới
            if (genreIds != null && genreIds.Any())
            {
                return AddMovieGenres(movieId, genreIds);
            }

            return true;
        }

        /// <summary>
        /// Kiểm tra phim có thể loại cụ thể không
        /// </summary>
        public bool HasMovieGenre(int movieId, int genreId)
        {
            string sql = "SELECT COUNT(*) FROM MovieCategory WHERE MovieID = @MovieId AND GenreID = @GenreId";
            using SqlConnection conn = DapperProvider.GetConnection();
            int count = conn.ExecuteScalar<int>(sql, new { MovieId = movieId, GenreId = genreId });
            return count > 0;
        }

        /// <summary>
        /// Đếm số phim có thể loại cụ thể
        /// </summary>
        public int CountMoviesByGenre(int genreId)
        {
            string sql = "SELECT COUNT(*) FROM MovieCategory WHERE GenreID = @GenreId";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.ExecuteScalar<int>(sql, new { GenreId = genreId });
        }
    }
}