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
        public IEnumerable<CountryDTO> GetAll()
        {
            string sql = "SELECT * FROM Countries ORDER BY CountryName ASC";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<CountryDTO>(sql);
        }

        /// <summary>
        /// Lấy quốc gia theo ID
        /// </summary>
        public CountryDTO? GetById(int countryId)
        {
            string sql = "SELECT * FROM Countries WHERE CountryID = @CountryId";
            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.QueryFirstOrDefault<CountryDTO>(sql, new { CountryId = countryId });
        }

        /// <summary>
        /// Kiểm tra tên quốc gia đã tồn tại chưa
        /// </summary>
        public bool IsCountryNameExists(string countryName)
        {
            string sql = "SELECT COUNT(*) FROM Countries WHERE CountryName = @CountryName";
            using SqlConnection conn = DapperProvider.GetConnection();
            int count = conn.ExecuteScalar<int>(sql, new { CountryName = countryName });
            return count > 0;
        }

        /// <summary>
        /// Thêm quốc gia mới
        /// </summary>
        public bool AddCountry(CountryDTO country)
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
    }
}