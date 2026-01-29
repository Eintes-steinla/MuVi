using Muvi.DAL;
using MuVi.DTO.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MuVi.BLL
{
    public class FavoriteBLL
    {
        private FavoriteDAL favoriteDAL = new FavoriteDAL();
        private UserDAL userDAL = new UserDAL();
        private MovieDAL movieDAL = new MovieDAL();

        private int currentPage = 1;
        private int pageSize = 10;

        // Search and filter parameters
        private string _searchKeyword = "";
        private string _movieTypeFilter = "Tất cả";
        private int? _yearFilter = null;

        /// <summary>
        /// Lấy danh sách yêu thích với filter
        /// </summary>
        public IEnumerable<FavoriteDTO> GetFavorites()
        {
            var allFavorites = favoriteDAL.GetAll();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(_searchKeyword))
            {
                allFavorites = allFavorites.Where(f =>
                    (f.Username?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (f.MovieTitle?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            // Apply movie type filter
            if (_movieTypeFilter != "Tất cả")
            {
                allFavorites = allFavorites.Where(f => f.MovieType == _movieTypeFilter);
            }

            // Apply year filter
            if (_yearFilter != null)
            {
                allFavorites = allFavorites.Where(f => f.ReleaseYear == _yearFilter);
            }

            // Apply pagination
            return allFavorites
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// Lấy tổng số trang
        /// </summary>
        public int GetTotalPages()
        {
            var allFavorites = favoriteDAL.GetAll();

            // Apply same filters
            if (!string.IsNullOrWhiteSpace(_searchKeyword))
            {
                allFavorites = allFavorites.Where(f =>
                    (f.Username?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (f.MovieTitle?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            if (_movieTypeFilter != "Tất cả")
            {
                allFavorites = allFavorites.Where(f => f.MovieType == _movieTypeFilter);
            }

            if (_yearFilter != null)
            {
                allFavorites = allFavorites.Where(f => f.ReleaseYear == _yearFilter);
            }

            int totalRecords = allFavorites.Count();
            return (int)Math.Ceiling(totalRecords / (double)pageSize);
        }

        public void SetSearchKeyword(string keyword)
        {
            _searchKeyword = keyword;
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

        public void ClearFilters()
        {
            _searchKeyword = "";
            _movieTypeFilter = "Tất cả";
            _yearFilter = null;
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
        /// Thêm phim vào yêu thích
        /// </summary>
        public bool AddFavorite(int userId, int movieId, out string message)
        {
            // Kiểm tra user có tồn tại không
            var user = userDAL.GetById(userId);
            if (user == null)
            {
                message = "Người dùng không tồn tại";
                return false;
            }

            // Kiểm tra phim có tồn tại không
            var movie = movieDAL.GetById(movieId);
            if (movie == null)
            {
                message = "Phim không tồn tại";
                return false;
            }

            // Kiểm tra đã yêu thích chưa
            if (favoriteDAL.IsFavoriteExists(userId, movieId))
            {
                message = "Phim đã có trong danh sách yêu thích";
                return false;
            }

            bool result = favoriteDAL.AddFavorite(userId, movieId);

            message = result ? "Thêm vào yêu thích thành công" : "Thêm vào yêu thích thất bại";
            return result;
        }

        /// <summary>
        /// Xóa khỏi danh sách yêu thích
        /// </summary>
        public bool DeleteFavorite(int userId, int movieId, out string message)
        {
            bool result = favoriteDAL.DeleteFavorite(userId, movieId);
            message = result ? "Xóa khỏi yêu thích thành công" : "Xóa khỏi yêu thích thất bại";
            return result;
        }

        /// <summary>
        /// Xóa nhiều yêu thích
        /// </summary>
        public bool DeleteMultipleFavorites(List<(int userId, int movieId)> favorites, out string message)
        {
            int successCount = 0;

            foreach (var (userId, movieId) in favorites)
            {
                if (favoriteDAL.DeleteFavorite(userId, movieId))
                {
                    successCount++;
                }
            }

            message = $"Đã xóa {successCount}/{favorites.Count} mục yêu thích";
            return successCount > 0;
        }

        /// <summary>
        /// Lấy danh sách user (để chọn khi thêm mới)
        /// </summary>
        public IEnumerable<UserDTO> GetAllUsers()
        {
            return userDAL.GetAll();
        }

        /// <summary>
        /// Lấy danh sách phim (để chọn khi thêm mới)
        /// </summary>
        public IEnumerable<MovieDTO> GetAllMovies()
        {
            return movieDAL.GetAll();
        }

        /// <summary>
        /// Lấy số lượng yêu thích của một phim
        /// </summary>
        public int GetFavoriteCountByMovie(int movieId)
        {
            return favoriteDAL.CountFavoritesByMovieId(movieId);
        }

        /// <summary>
        /// Lấy số lượng yêu thích của một user
        /// </summary>
        public int GetFavoriteCountByUser(int userId)
        {
            return favoriteDAL.CountFavoritesByUserId(userId);
        }
    }
}