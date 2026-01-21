using Dapper;
using Microsoft.Data.SqlClient;
using MuVi.DAL;
using MuVi.DTO.DTOs;

namespace Muvi.DAL
{
    public class CountryDAL
    {
        /// <summary>
        /// Lấy toàn bộ danh sách quốc gia
        /// </summary>
        public List<CountryDTO> GetAll()
        {
            string sql = "SELECT * FROM Countries ORDER BY CountryName";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<CountryDTO>(sql).ToList();
        }

        /// <summary>
        /// Lấy thông tin quốc gia theo ID
        /// </summary>
        public CountryDTO? GetById(int countryId)
        {
            string sql = "SELECT * FROM Countries WHERE CountryID = @Id";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.QueryFirstOrDefault<CountryDTO>(sql, new { Id = countryId });
        }

        /// <summary>
        /// Kiểm tra tên quốc gia đã tồn tại chưa (tránh trùng lặp)
        /// </summary>
        public bool IsCountryNameExists(string countryName)
        {
            string sql = "SELECT COUNT(*) FROM Countries WHERE CountryName = @Name";

            using SqlConnection conn = DapperProvider.GetConnection();
            int count = conn.ExecuteScalar<int>(sql, new { Name = countryName });

            return count > 0;
        }

        /// <summary>
        /// Thêm quốc gia mới
        /// </summary>
        public bool Insert(CountryDTO country)
        {
            string sql = @"
                INSERT INTO Countries (CountryName, CountryCode) 
                VALUES (@CountryName, @CountryCode)";

            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new
            {
                country.CountryName,
                country.CountryCode
            });

            return rows > 0;
        }

        /// <summary>
        /// Cập nhật quốc gia
        /// </summary>
        public bool Update(CountryDTO country)
        {
            string sql = @"
                UPDATE Countries 
                SET CountryName = @CountryName, 
                    CountryCode = @CountryCode 
                WHERE CountryID = @CountryID";

            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, country);

            return rows > 0;
        }

        /// <summary>
        /// Xóa quốc gia
        /// </summary>
        public bool Delete(int countryId)
        {
            string sql = "DELETE FROM Countries WHERE CountryID = @Id";

            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new { Id = countryId });

            return rows > 0;
        }
    }
}