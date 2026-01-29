using Dapper;
using Microsoft.Data.SqlClient;
using MuVi.DAL;
using MuVi.DTO.DTOs;
using System.Collections.Generic;

namespace Muvi.DAL
{
    public class CountryDAL
    {
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
        /// Kiểm tra tên quốc gia đã tồn tại chưa (trừ quốc gia hiện tại)
        /// </summary>
        public bool IsCountryNameExists(string countryName, int countryId)
        {
            string sql = "SELECT COUNT(*) FROM Countries WHERE CountryName = @CountryName AND CountryID != @CountryID";
            using SqlConnection conn = DapperProvider.GetConnection();
            int count = conn.ExecuteScalar<int>(sql, new { CountryName = countryName, CountryID = countryId });
            return count > 0;
        }

        /// <summary>
        /// Kiểm tra mã quốc gia đã tồn tại chưa
        /// </summary>
        public bool IsCountryCodeExists(string countryCode)
        {
            string sql = "SELECT COUNT(*) FROM Countries WHERE CountryCode = @CountryCode";
            using SqlConnection conn = DapperProvider.GetConnection();
            int count = conn.ExecuteScalar<int>(sql, new { CountryCode = countryCode });
            return count > 0;
        }

        /// <summary>
        /// Kiểm tra mã quốc gia đã tồn tại chưa (trừ quốc gia hiện tại)
        /// </summary>
        public bool IsCountryCodeExists(string countryCode, int countryId)
        {
            string sql = "SELECT COUNT(*) FROM Countries WHERE CountryCode = @CountryCode AND CountryID != @CountryID";
            using SqlConnection conn = DapperProvider.GetConnection();
            int count = conn.ExecuteScalar<int>(sql, new { CountryCode = countryCode, CountryID = countryId });
            return count > 0;
        }

        /// <summary>
        /// Thêm quốc gia mới
        /// </summary>
        public int AddCountry(CountryDTO country)
        {
            string sql = @"
            INSERT INTO Countries
            (
                CountryName,
                CountryCode
            )
            VALUES
            (
                @CountryName,
                @CountryCode
            );
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using SqlConnection conn = DapperProvider.GetConnection();
            int countryId = conn.ExecuteScalar<int>(sql, new
            {
                country.CountryName,
                country.CountryCode
            });

            return countryId;
        }

        /// <summary>
        /// Cập nhật thông tin quốc gia
        /// </summary>
        public bool UpdateCountry(CountryDTO country)
        {
            string sql = @"
            UPDATE Countries 
            SET 
                CountryName = @CountryName,
                CountryCode = @CountryCode
            WHERE CountryID = @CountryID";

            using SqlConnection conn = DapperProvider.GetConnection();
            int rows = conn.Execute(sql, new
            {
                country.CountryID,
                country.CountryName,
                country.CountryCode
            });

            return rows > 0;
        }

        /// <summary>
        /// Lấy quốc gia theo ID
        /// </summary>
        public CountryDTO? GetById(int countryId)
        {
            string sql = @"
            SELECT CountryID, CountryName, CountryCode
            FROM Countries
            WHERE CountryID = @CountryId";

            using SqlConnection conn = DapperProvider.GetConnection();
            var country = conn.QueryFirstOrDefault<CountryDTO>(sql, new { CountryId = countryId });

            return country;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách quốc gia
        /// </summary>
        public IEnumerable<CountryDTO> GetAll()
        {
            string sql = @"
            SELECT CountryID, CountryName, CountryCode
            FROM Countries
            ORDER BY CountryName ASC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<CountryDTO>(sql);
        }

        /// <summary>
        /// Lấy quốc gia phân trang
        /// </summary>
        public IEnumerable<CountryDTO> GetCountriesPaged(int pageNumber, int pageSize)
        {
            string sql = @"
            SELECT CountryID, CountryName, CountryCode
            FROM Countries
            ORDER BY CountryID ASC
            OFFSET @Offset ROWS 
            FETCH NEXT @PageSize ROWS ONLY";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<CountryDTO>(sql, new
            {
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize
            });
        }

        /// <summary>
        /// Tìm kiếm quốc gia
        /// </summary>
        public IEnumerable<CountryDTO> SearchCountries(string keyword)
        {
            string sql = @"
            SELECT CountryID, CountryName, CountryCode
            FROM Countries
            WHERE CountryName LIKE @Key OR CountryCode LIKE @Key
            ORDER BY CountryName ASC";

            using SqlConnection conn = DapperProvider.GetConnection();
            return conn.Query<CountryDTO>(sql, new { Key = $"%{keyword}%" });
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

        /// <summary>
        /// Kiểm tra quốc gia có đang được sử dụng trong bảng Movies không
        /// </summary>
        public bool IsCountryInUse(int countryId)
        {
            string sql = "SELECT COUNT(*) FROM Movies WHERE CountryID = @CountryID";
            using SqlConnection conn = DapperProvider.GetConnection();
            int count = conn.ExecuteScalar<int>(sql, new { CountryID = countryId });
            return count > 0;
        }
    }
}