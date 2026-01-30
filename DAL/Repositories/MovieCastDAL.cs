using Dapper;
using Microsoft.Data.SqlClient;
using MuVi.DAL;
using MuVi.DTO.DTOs;

namespace Muvi.DAL
{
    public class MovieCastDAL
    {
        /// <summary>
        /// Kiểm tra diễn viên đã tham gia phim chưa
        /// </summary>
        public bool HasActorInMovie(int movieId, int actorId)
        {
            string sql = "SELECT COUNT(*) FROM MovieCast WHERE MovieID = @MovieId AND ActorID = @ActorId";
            using SqlConnection conn = DapperProvider.GetConnection();
            int count = conn.ExecuteScalar<int>(sql, new { MovieId = movieId, ActorId = actorId });
            return count > 0;
        }

        /// <summary>
        /// Thêm diễn viên vào phim
        /// </summary>
        public bool AddMovieCast(MovieCastDTO cast)
        {
            string sql = @"
            INSERT INTO MovieCast
            (MovieID, ActorID, RoleName, [Order])
            VALUES
            (@MovieID, @ActorID, @RoleName, @Order)";

            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new
            {
                cast.MovieID,
                cast.ActorID,
                cast.RoleName,
                cast.Order
            });

            return rows > 0;
        }

        /// <summary>
        /// Cập nhật thông tin diễn viên trong phim
        /// </summary>
        public bool UpdateMovieCast(MovieCastDTO cast)
        {
            string sql = @"
            UPDATE MovieCast 
            SET 
                RoleName = @RoleName,
                [Order] = @Order
            WHERE MovieID = @MovieID AND ActorID = @ActorID";

            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new
            {
                cast.MovieID,
                cast.ActorID,
                cast.RoleName,
                cast.Order
            });

            return rows > 0;
        }

        /// <summary>
        /// Lấy thông tin diễn viên trong phim
        /// </summary>
        public MovieCastDTO? GetByMovieAndActor(int movieId, int actorId)
        {
            string sql = @"
            SELECT 
                mc.*,
                m.Title AS MovieTitle,
                a.ActorName
            FROM MovieCast mc
            INNER JOIN Movies m ON mc.MovieID = m.MovieID
            INNER JOIN Actors a ON mc.ActorID = a.ActorID
            WHERE mc.MovieID = @MovieId AND mc.ActorID = @ActorId";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.QueryFirstOrDefault<MovieCastDTO>(sql, new { MovieId = movieId, ActorId = actorId });
        }

        /// <summary>
        /// Lấy toàn bộ danh sách MovieCast
        /// </summary>
        public IEnumerable<MovieCastDTO> GetAll()
        {
            string sql = @"
            SELECT 
                mc.*,
                m.Title AS MovieTitle,
                a.ActorName
            FROM MovieCast mc
            INNER JOIN Movies m ON mc.MovieID = m.MovieID
            INNER JOIN Actors a ON mc.ActorID = a.ActorID
            ORDER BY m.Title, mc.[Order] ASC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<MovieCastDTO>(sql);
        }

        /// <summary>
        /// Lấy danh sách diễn viên của một phim
        /// </summary>
        public IEnumerable<MovieCastDTO> GetByMovieId(int movieId)
        {
            string sql = @"
            SELECT 
                mc.*,
                m.Title AS MovieTitle,
                a.ActorName
            FROM MovieCast mc
            INNER JOIN Movies m ON mc.MovieID = m.MovieID
            INNER JOIN Actors a ON mc.ActorID = a.ActorID
            WHERE mc.MovieID = @MovieId
            ORDER BY mc.[Order] ASC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<MovieCastDTO>(sql, new { MovieId = movieId });
        }

        /// <summary>
        /// Lấy danh sách phim của một diễn viên
        /// </summary>
        public IEnumerable<MovieCastDTO> GetByActorId(int actorId)
        {
            string sql = @"
            SELECT 
                mc.*,
                m.Title AS MovieTitle,
                a.ActorName
            FROM MovieCast mc
            INNER JOIN Movies m ON mc.MovieID = m.MovieID
            INNER JOIN Actors a ON mc.ActorID = a.ActorID
            WHERE mc.ActorID = @ActorId
            ORDER BY m.Title";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<MovieCastDTO>(sql, new { ActorId = actorId });
        }

        /// <summary>
        /// Tìm kiếm MovieCast
        /// </summary>
        public IEnumerable<MovieCastDTO> Search(string keyword)
        {
            string sql = @"
            SELECT 
                mc.*,
                m.Title AS MovieTitle,
                a.ActorName
            FROM MovieCast mc
            INNER JOIN Movies m ON mc.MovieID = m.MovieID
            INNER JOIN Actors a ON mc.ActorID = a.ActorID
            WHERE m.Title LIKE @Key 
            OR a.ActorName LIKE @Key 
            OR mc.RoleName LIKE @Key
            ORDER BY m.Title, mc.[Order] ASC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<MovieCastDTO>(sql, new { Key = $"%{keyword}%" });
        }

        /// <summary>
        /// Xóa diễn viên khỏi phim
        /// </summary>
        public bool Delete(int movieId, int actorId)
        {
            string sql = "DELETE FROM MovieCast WHERE MovieID = @MovieId AND ActorID = @ActorId";
            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new { MovieId = movieId, ActorId = actorId });
            return rows > 0;
        }

        /// <summary>
        /// Xóa tất cả diễn viên của phim
        /// </summary>
        public bool DeleteByMovieId(int movieId)
        {
            string sql = "DELETE FROM MovieCast WHERE MovieID = @MovieId";
            using SqlConnection conn = DapperProvider.GetConnection();
            conn.Execute(sql, new { MovieId = movieId });
            return true;
        }

        /// <summary>
        /// Xóa tất cả phim của diễn viên
        /// </summary>
        public bool DeleteByActorId(int actorId)
        {
            string sql = "DELETE FROM MovieCast WHERE ActorID = @ActorId";
            using SqlConnection conn = DapperProvider.GetConnection();
            conn.Execute(sql, new { ActorId = actorId });
            return true;
        }

        /// <summary>
        /// Đếm số diễn viên trong phim
        /// </summary>
        public int CountActorsInMovie(int movieId)
        {
            string sql = "SELECT COUNT(*) FROM MovieCast WHERE MovieID = @MovieId";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.ExecuteScalar<int>(sql, new { MovieId = movieId });
        }

        /// <summary>
        /// Đếm số phim của diễn viên
        /// </summary>
        public int CountMoviesOfActor(int actorId)
        {
            string sql = "SELECT COUNT(*) FROM MovieCast WHERE ActorID = @ActorId";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.ExecuteScalar<int>(sql, new { ActorId = actorId });
        }
    }
}