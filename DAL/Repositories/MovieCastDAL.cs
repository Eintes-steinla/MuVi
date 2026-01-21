using Dapper;
using Microsoft.Data.SqlClient;
using MuVi.DAL;
using MuVi.DTO.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Muvi.DAL
{
    public class MovieCastDAL
    {
        /// <summary>
        /// Lấy danh sách diễn viên và vai diễn của một bộ phim cụ thể
        /// </summary>
        public List<MovieCastDTO> GetCastByMovie(int movieId)
        {
            // JOIN với bảng Actors để lấy ActorName
            string sql = @"
                SELECT mc.*, a.ActorName 
                FROM MovieCast mc
                JOIN Actors a ON mc.ActorID = a.ActorID
                WHERE mc.MovieID = @MovieID
                ORDER BY mc.[Order] ASC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<MovieCastDTO>(sql, new { MovieID = movieId }).ToList();
        }

        /// <summary>
        /// Thêm diễn viên vào phim
        /// </summary>
        public bool Insert(MovieCastDTO cast)
        {
            string sql = @"
                INSERT INTO MovieCast (MovieID, ActorID, RoleName, [Order])
                VALUES (@MovieID, @ActorID, @RoleName, @Order)";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Execute(sql, cast) > 0;
        }

        /// <summary>
        /// Xóa một diễn viên khỏi bộ phim
        /// </summary>
        public bool Delete(int movieId, int actorId)
        {
            string sql = "DELETE FROM MovieCast WHERE MovieID = @MovieID AND ActorID = @ActorID";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Execute(sql, new { MovieID = movieId, ActorID = actorId }) > 0;
        }
    }
}