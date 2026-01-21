using Dapper;
using Microsoft.Data.SqlClient;
using MuVi.DAL;
using MuVi.DTO.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Muvi.DAL
{
    public class MovieDAL
    {
        public List<MovieDTO> GetAll()
        {
            string sql = "SELECT * FROM Movies ORDER BY CreatedAt DESC";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<MovieDTO>(sql).ToList();
        }

        public MovieDTO? GetById(int id)
        {
            string sql = "SELECT * FROM Movies WHERE MovieID = @Id";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.QueryFirstOrDefault<MovieDTO>(sql, new { Id = id });
        }

        public bool Insert(MovieDTO movie)
        {
            string sql = @"
                INSERT INTO Movies (Title, MovieType, CountryID, Director, ReleaseYear, 
                                    Description, PosterPath, TrailerURL, Duration, 
                                    TotalEpisodes, Status, CreatedAt, UpdatedAt)
                VALUES (@Title, @MovieType, @CountryID, @Director, @ReleaseYear, 
                        @Description, @PosterPath, @TrailerUrl, @Duration, 
                        @TotalEpisodes, @Status, GETDATE(), GETDATE())";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Execute(sql, movie) > 0;
        }

        public bool Update(MovieDTO movie)
        {
            string sql = @"
                UPDATE Movies 
                SET Title = @Title, MovieType = @MovieType, CountryID = @CountryID, 
                    Director = @Director, ReleaseYear = @ReleaseYear, 
                    Description = @Description, PosterPath = @PosterPath, 
                    TrailerURL = @TrailerUrl, Duration = @Duration, 
                    TotalEpisodes = @TotalEpisodes, Status = @Status, 
                    UpdatedAt = GETDATE()
                WHERE MovieID = @MovieID";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Execute(sql, movie) > 0;
        }

        public bool Delete(int id)
        {
            string sql = "DELETE FROM Movies WHERE MovieID = @Id";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Execute(sql, new { Id = id }) > 0;
        }

        // Hàm tìm kiếm phim theo tên
        public List<MovieDTO> SearchByTitle(string title)
        {
            string sql = "SELECT * FROM Movies WHERE Title LIKE @Title";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<MovieDTO>(sql, new { Title = "%" + title + "%" }).ToList();
        }
    }
}