using Muvi.DAL;
using MuVi.DTO.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MuVi.BLL
{
    public class GenreBLL
    {
        private GenreDAL genreDAL = new GenreDAL();

        private int currentPage = 1;
        private int pageSize = 10;

        // Search parameter
        private string _searchKeyword = "";

        /// <summary>
        /// Lấy danh sách thể loại với filter
        /// </summary>
        public IEnumerable<GenreDTO> GetGenres()
        {
            var allGenres = genreDAL.GetAll();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(_searchKeyword))
            {
                allGenres = allGenres.Where(g =>
                    (g.GenreName?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (g.Description?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            // Apply pagination
            return allGenres
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// Lấy tổng số trang
        /// </summary>
        public int GetTotalPages()
        {
            var allGenres = genreDAL.GetAll();

            // Apply same filters
            if (!string.IsNullOrWhiteSpace(_searchKeyword))
            {
                allGenres = allGenres.Where(g =>
                    (g.GenreName?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (g.Description?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            int totalRecords = allGenres.Count();
            return (int)Math.Ceiling(totalRecords / (double)pageSize);
        }

        public void SetSearchKeyword(string keyword)
        {
            _searchKeyword = keyword;
            currentPage = 1;
        }

        public void ClearFilters()
        {
            _searchKeyword = "";
            currentPage = 1;
        }

        public void NextPage()
        {
            int totalPages = GetTotalPages();
            if (currentPage < totalPages)
            {
                currentPage++;
            }
        }

        public void PreviousPage()
        {
            if (currentPage > 1)
            {
                currentPage--;
            }
        }

        public void FirstPage()
        {
            currentPage = 1;
        }

        public void LastPage()
        {
            currentPage = GetTotalPages();
        }

        public int GetCurrentPage() => currentPage;

        /// <summary>
        /// Thêm thể loại mới
        /// </summary>
        public bool AddGenre(GenreDTO genre, out string message)
        {
            if (genreDAL.IsGenreNameExists(genre.GenreName))
            {
                message = "Tên thể loại đã tồn tại";
                return false;
            }

            bool result = genreDAL.AddGenre(genre);
            message = result ? "Thêm thể loại thành công" : "Thêm thể loại thất bại";
            return result;
        }

        /// <summary>
        /// Cập nhật thể loại
        /// </summary>
        public bool UpdateGenre(GenreDTO genre, out string message)
        {
            if (genreDAL.IsGenreNameExists(genre.GenreName, genre.GenreID))
            {
                message = "Tên thể loại đã tồn tại";
                return false;
            }

            bool result = genreDAL.UpdateGenre(genre);
            message = result ? "Cập nhật thể loại thành công" : "Cập nhật thể loại thất bại";
            return result;
        }

        /// <summary>
        /// Xóa thể loại
        /// </summary>
        public bool DeleteGenre(int genreId, out string message)
        {
            bool result = genreDAL.Delete(genreId);
            message = result ? "Xóa thể loại thành công" : "Xóa thể loại thất bại";
            return result;
        }

        /// <summary>
        /// Xóa nhiều thể loại
        /// </summary>
        public bool DeleteMultipleGenres(List<int> genreIds, out string message)
        {
            int successCount = 0;
            foreach (int genreId in genreIds)
            {
                if (genreDAL.Delete(genreId))
                {
                    successCount++;
                }
            }

            message = $"Đã xóa {successCount}/{genreIds.Count} thể loại";
            return successCount > 0;
        }

        // Thêm phương thức này vào GenreBLL.cs
        public List<GenreDTO> GetAllGenres(out string message)
        {
            try
            {
                var genres = genreDAL.GetAll().ToList();
                message = "Thành công";
                return genres;
            }
            catch (Exception ex)
            {
                message = $"Lỗi: {ex.Message}";
                return new List<GenreDTO>();
            }
        }
    }
}