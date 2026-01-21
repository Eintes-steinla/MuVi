using Dapper;
using Microsoft.Data.SqlClient;
using MuVi.DAL;
using MuVi.DTO.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Muvi.DAL
{
    public class GenreDAL
    {
        public List<GenreDTO> GetAll()
        {
            string sql = "SELECT GenreID, GenreName, Description FROM Genres ORDER BY GenreName";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<GenreDTO>(sql).ToList();
        }

        public GenreDTO? GetById(int id)
        {
            string sql = "SELECT GenreID, GenreName, Description FROM Genres WHERE GenreID = @Id";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.QueryFirstOrDefault<GenreDTO>(sql, new { Id = id });
        }

        public bool IsGenreNameExists(string name)
        {
            string sql = "SELECT COUNT(*) FROM Genres WHERE GenreName = @GenreName";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.ExecuteScalar<int>(sql, new { Name = name }) > 0;
        }

        public bool Insert(GenreDTO genre)
        {
            string sql = "INSERT INTO Genres (GenreName, Description) VALUES (@GenreName, @Description)";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Execute(sql, genre) > 0;
        }

        public bool Update(GenreDTO genre)
        {
            string sql = "UPDATE Genres SET GenreName = @GenreName, Description = @Description WHERE GenreID = @GenreID";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Execute(sql, genre) > 0;
        }

        public bool Delete(int id)
        {
            string sql = "DELETE FROM Genres WHERE GenreID = @Id";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Execute(sql, new { Id = id }) > 0;
        }
    }
}