using Dapper;
using Microsoft.Data.SqlClient;
using MuVi.DAL;
using MuVi.DTO.DTOs;

namespace Muvi.DAL
{
    public class ActorDAL
    {
        /// <summary>
        /// Kiểm tra tên diễn viên đã tồn tại chưa
        /// </summary>
        public bool IsActorNameExists(string actorName)
        {
            string sql = "SELECT COUNT(*) FROM Actors WHERE ActorName = @ActorName";
            using SqlConnection conn = DapperProvider.GetConnection();
            int count = conn.ExecuteScalar<int>(sql, new { ActorName = actorName });
            return count > 0;
        }

        /// <summary>
        /// Kiểm tra tên diễn viên đã tồn tại chưa (trừ actor hiện tại)
        /// </summary>
        public bool IsActorNameExists(string actorName, int actorId)
        {
            string sql = "SELECT COUNT(*) FROM Actors WHERE ActorName = @ActorName AND ActorID != @ActorID";
            using SqlConnection conn = DapperProvider.GetConnection();
            int count = conn.ExecuteScalar<int>(sql, new { ActorName = actorName, ActorID = actorId });
            return count > 0;
        }

        /// <summary>
        /// Thêm diễn viên mới
        /// </summary>
        public bool AddActor(ActorDTO actor)
        {
            string sql = @"
            INSERT INTO Actors
            (
                ActorName,
                Bio,
                PhotoPath,
                DateOfBirth,
                Nationality,
                CreatedAt
            )
            VALUES
            (
                @ActorName,
                @Bio,
                @PhotoPath,
                @DateOfBirth,
                @Nationality,
                GETDATE()
            )";

            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new
            {
                actor.ActorName,
                actor.Bio,
                actor.PhotoPath,
                actor.DateOfBirth,
                actor.Nationality
            });

            return rows > 0;
        }

        /// <summary>
        /// Cập nhật thông tin diễn viên
        /// </summary>
        public bool UpdateActor(ActorDTO actor)
        {
            string sql = @"
            UPDATE Actors 
            SET 
                ActorName = @ActorName,
                Bio = @Bio,
                PhotoPath = @PhotoPath,
                DateOfBirth = @DateOfBirth,
                Nationality = @Nationality
            WHERE ActorID = @ActorID";

            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new
            {
                actor.ActorID,
                actor.ActorName,
                actor.Bio,
                actor.PhotoPath,
                actor.DateOfBirth,
                actor.Nationality
            });

            return rows > 0;
        }

        /// <summary>
        /// Lấy diễn viên theo ID
        /// </summary>
        public ActorDTO? GetById(int actorId)
        {
            string sql = "SELECT * FROM Actors WHERE ActorID = @ActorId";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.QueryFirstOrDefault<ActorDTO>(sql, new { ActorId = actorId });
        }

        /// <summary>
        /// Lấy toàn bộ danh sách diễn viên
        /// </summary>
        public IEnumerable<ActorDTO> GetAll()
        {
            string sql = "SELECT * FROM Actors ORDER BY CreatedAt DESC";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<ActorDTO>(sql);
        }

        /// <summary>
        /// Lấy diễn viên phân trang
        /// </summary>
        public IEnumerable<ActorDTO> GetActorsPaged(int pageNumber, int pageSize)
        {
            string sql = @"
            SELECT *
            FROM Actors
            ORDER BY ActorID ASC
            OFFSET @Offset ROWS 
            FETCH NEXT @PageSize ROWS ONLY";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<ActorDTO>(sql, new
            {
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize
            });
        }

        /// <summary>
        /// Tìm kiếm diễn viên
        /// </summary>
        public IEnumerable<ActorDTO> SearchActors(string keyword)
        {
            string sql = @"
            SELECT *
            FROM Actors
            WHERE ActorName LIKE @Key OR Nationality LIKE @Key OR Bio LIKE @Key
            ORDER BY CreatedAt DESC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<ActorDTO>(sql, new { Key = $"%{keyword}%" });
        }

        /// <summary>
        /// Xóa diễn viên
        /// </summary>
        public bool Delete(int actorId)
        {
            string sql = "DELETE FROM Actors WHERE ActorID = @Id";
            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new { Id = actorId });
            return rows > 0;
        }
    }
}