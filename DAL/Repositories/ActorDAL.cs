using Dapper;
using Microsoft.Data.SqlClient;
using MuVi.DAL;
using MuVi.DTO.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Muvi.DAL
{
    public class ActorDAL
    {
        public List<ActorDTO> GetAll()
        {
            string sql = "SELECT * FROM Actors ORDER BY ActorName ASC";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<ActorDTO>(sql).ToList();
        }

        public ActorDTO? GetById(int id)
        {
            string sql = "SELECT * FROM Actors WHERE ActorID = @Id";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.QueryFirstOrDefault<ActorDTO>(sql, new { Id = id });
        }

        public bool Insert(ActorDTO actor)
        {
            string sql = @"
                INSERT INTO Actors (ActorName, Bio, PhotoPath, DateOfBirth, Nationality, CreatedAt)
                VALUES (@ActorName, @Bio, @PhotoPath, @DateOfBirth, @Nationality, GETDATE())";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Execute(sql, actor) > 0;
        }

        public bool Update(ActorDTO actor)
        {
            string sql = @"
                UPDATE Actors 
                SET ActorName = @ActorName, 
                    Bio = @Bio, 
                    PhotoPath = @PhotoPath, 
                    DateOfBirth = @DateOfBirth, 
                    Nationality = @Nationality
                WHERE ActorID = @ActorID";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Execute(sql, actor) > 0;
        }

        public bool Delete(int id)
        {
            string sql = "DELETE FROM Actors WHERE ActorID = @Id";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Execute(sql, new { Id = id }) > 0;
        }

        public List<ActorDTO> SearchByName(string name)
        {
            string sql = "SELECT * FROM Actors WHERE ActorName LIKE @Name";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<ActorDTO>(sql, new { Name = "%" + name + "%" }).ToList();
        }
    }
}