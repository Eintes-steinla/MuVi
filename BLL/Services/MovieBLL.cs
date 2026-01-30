using Muvi.DAL;
using MuVi.DTO.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MuVi.BLL
{
    public class MovieBLL
    {
        private MovieDAL movieDAL = new MovieDAL();
        private CountryDAL countryDAL = new CountryDAL();
        private GenreDAL genreDAL = new GenreDAL();

        private int currentPage = 1;
        private int pageSize = 10;

        // Search and filter parameters
        private string _searchKeyword = "";
        private string _statusFilter = "Tất cả";
        private string _movieTypeFilter = "Tất cả";
        private int? _yearFilter = null;
        private int? _countryFilter = null;

        /// <summary>
        /// Lấy danh sách phim với filter
        /// </summary>
        public IEnumerable<MovieDTO> GetMovies()
        {
            var allMovies = movieDAL.GetAll();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(_searchKeyword))
            {
                allMovies = allMovies.Where(m =>
                    (m.Title?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (m.Director?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (m.CountryName?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            // Apply status filter
            if (_statusFilter != "Tất cả")
            {
                allMovies = allMovies.Where(m => m.Status == _statusFilter);
            }

            // Apply movie type filter
            if (_movieTypeFilter != "Tất cả")
            {
                allMovies = allMovies.Where(m => m.MovieType == _movieTypeFilter);
            }

            // Apply year filter
            if (_yearFilter != null)
            {
                allMovies = allMovies.Where(m => m.ReleaseYear == _yearFilter);
            }

            // Apply country filter
            if (_countryFilter != null)
            {
                allMovies = allMovies.Where(m => m.CountryID == _countryFilter);
            }

            // Apply pagination
            return allMovies
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// Lấy toàn bộ danh sách phim (không phân trang) để dùng cho trang chủ
        /// </summary>
        public List<MovieDTO> GetAllMovies(out string message)
        {
            try
            {
                var movies = movieDAL.GetAll().ToList();
                message = "Thành công";
                return movies;
            }
            catch (Exception ex)
            {
                message = $"Lỗi: {ex.Message}";
                return new List<MovieDTO>();
            }
        }

        /// <summary>
        /// Lấy tổng số trang
        /// </summary>
        public int GetTotalPages()
        {
            var allMovies = movieDAL.GetAll();

            // Apply same filters
            if (!string.IsNullOrWhiteSpace(_searchKeyword))
            {
                allMovies = allMovies.Where(m =>
                    (m.Title?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (m.Director?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (m.CountryName?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            if (_statusFilter != "Tất cả")
            {
                allMovies = allMovies.Where(m => m.Status == _statusFilter);
            }

            if (_movieTypeFilter != "Tất cả")
            {
                allMovies = allMovies.Where(m => m.MovieType == _movieTypeFilter);
            }

            if (_yearFilter != null)
            {
                allMovies = allMovies.Where(m => m.ReleaseYear == _yearFilter);
            }

            if (_countryFilter != null)
            {
                allMovies = allMovies.Where(m => m.CountryID == _countryFilter);
            }

            int totalRecords = allMovies.Count();
            return (int)Math.Ceiling(totalRecords / (double)pageSize);
        }

        public void SetSearchKeyword(string keyword)
        {
            _searchKeyword = keyword;
            currentPage = 1;
        }

        public void SetStatusFilter(string status)
        {
            _statusFilter = status;
            currentPage = 1;
        }

        public void SetMovieTypeFilter(string movieType)
        {
            _movieTypeFilter = movieType;
            currentPage = 1;
        }

        public void SetYearFilter(int? year)
        {
            _yearFilter = year;
            currentPage = 1;
        }

        public void SetCountryFilter(int? countryId)
        {
            _countryFilter = countryId;
            currentPage = 1;
        }

        public void ClearFilters()
        {
            _searchKeyword = "";
            _statusFilter = "Tất cả";
            _movieTypeFilter = "Tất cả";
            _yearFilter = null;
            _countryFilter = null;
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
        /// Thêm phim mới (bao gồm thể loại)
        /// </summary>
        public bool AddMovie(MovieDTO movie, List<int> genreIds, out string message)
        {
            if (movieDAL.IsTitleExists(movie.Title))
            {
                message = "Tiêu đề phim đã tồn tại";
                return false;
            }

            // Thêm phim và lấy MovieID
            int movieId = movieDAL.AddMovie(movie);

            if (movieId > 0)
            {
                // Thêm thể loại cho phim
                if (genreIds != null && genreIds.Any())
                {
                    foreach (int genreId in genreIds)
                    {
                        genreDAL.AddMovieGenre(movieId, genreId);
                    }
                }

                message = "Thêm phim thành công";
                return true;
            }

            message = "Thêm phim thất bại";
            return false;
        }

        /// <summary>
        /// Cập nhật phim (bao gồm thể loại)
        /// </summary>
        public bool UpdateMovie(MovieDTO movie, List<int> genreIds, out string message)
        {
            if (movieDAL.IsTitleExists(movie.Title, movie.MovieID))
            {
                message = "Tiêu đề phim đã tồn tại";
                return false;
            }

            bool result = movieDAL.UpdateMovie(movie);

            if (result)
            {
                // Xóa tất cả thể loại cũ
                genreDAL.DeleteMovieGenres(movie.MovieID);

                // Thêm thể loại mới
                if (genreIds != null && genreIds.Any())
                {
                    foreach (int genreId in genreIds)
                    {
                        genreDAL.AddMovieGenre(movie.MovieID, genreId);
                    }
                }

                message = "Cập nhật phim thành công";
                return true;
            }

            message = "Cập nhật phim thất bại";
            return false;
        }

        /// <summary>
        /// Xóa phim
        /// </summary>
        public bool DeleteMovie(int movieId, out string message)
        {
            bool result = movieDAL.Delete(movieId);
            message = result ? "Xóa phim thành công" : "Xóa phim thất bại";
            return result;
        }

        /// <summary>
        /// Xóa nhiều phim
        /// </summary>
        public bool DeleteMultipleMovies(List<int> movieIds, out string message)
        {
            int successCount = 0;
            foreach (int movieId in movieIds)
            {
                if (movieDAL.Delete(movieId))
                {
                    successCount++;
                }
            }

            message = $"Đã xóa {successCount}/{movieIds.Count} phim";
            return successCount > 0;
        }

        /// <summary>
        /// Lấy danh sách quốc gia
        /// </summary>
        public IEnumerable<CountryDTO> GetAllCountries()
        {
            return countryDAL.GetAll();
        }

        /// <summary>
        /// Lấy danh sách thể loại
        /// </summary>
        public IEnumerable<GenreDTO> GetAllGenres()
        {
            return genreDAL.GetAll();
        }

        /// <summary>
        /// Lấy thông tin phim theo ID (kèm thể loại)
        /// </summary>
        public MovieDTO? GetMovieById(int movieId)
        {
            return movieDAL.GetById(movieId);
        }
    }
}