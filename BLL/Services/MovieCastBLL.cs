using Muvi.DAL;
using MuVi.DTO.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MuVi.BLL
{
    public class MovieCastBLL
    {
        private MovieCastDAL castDAL = new MovieCastDAL();
        private ActorDAL actorDAL = new ActorDAL();
        private MovieDAL movieDAL = new MovieDAL();

        private int currentPage = 1;
        private int pageSize = 10;

        // Filter parameters
        private string _searchKeyword = "";
        private int? _movieFilter = null;
        private int? _actorFilter = null;
        private int? _orderFilter = null;  // Filter theo vai chính/phụ

        /// <summary>
        /// Lấy danh sách MovieCast với filter và phân trang
        /// </summary>
        public IEnumerable<MovieCastDTO> GetMovieCasts()
        {
            var allCasts = castDAL.GetAll();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(_searchKeyword))
            {
                allCasts = allCasts.Where(c =>
                    (c.MovieTitle?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (c.ActorName?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (c.RoleName?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            // Apply movie filter
            if (_movieFilter.HasValue)
            {
                allCasts = allCasts.Where(c => c.MovieID == _movieFilter.Value);
            }

            // Apply actor filter
            if (_actorFilter.HasValue)
            {
                allCasts = allCasts.Where(c => c.ActorID == _actorFilter.Value);
            }

            // Apply order filter (vai chính = 1, vai phụ = 2+)
            if (_orderFilter.HasValue)
            {
                if (_orderFilter.Value == 1)
                {
                    allCasts = allCasts.Where(c => c.Order == 1);
                }
                else if (_orderFilter.Value == 2)
                {
                    allCasts = allCasts.Where(c => c.Order > 1);
                }
            }

            // Apply pagination
            return allCasts
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// Lấy tổng số trang
        /// </summary>
        public int GetTotalPages()
        {
            var allCasts = castDAL.GetAll();

            // Apply same filters
            if (!string.IsNullOrWhiteSpace(_searchKeyword))
            {
                allCasts = allCasts.Where(c =>
                    (c.MovieTitle?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (c.ActorName?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (c.RoleName?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            if (_movieFilter.HasValue)
            {
                allCasts = allCasts.Where(c => c.MovieID == _movieFilter.Value);
            }

            if (_actorFilter.HasValue)
            {
                allCasts = allCasts.Where(c => c.ActorID == _actorFilter.Value);
            }

            if (_orderFilter.HasValue)
            {
                if (_orderFilter.Value == 1)
                {
                    allCasts = allCasts.Where(c => c.Order == 1);
                }
                else if (_orderFilter.Value == 2)
                {
                    allCasts = allCasts.Where(c => c.Order > 1);
                }
            }

            int totalRecords = allCasts.Count();
            return (int)Math.Ceiling(totalRecords / (double)pageSize);
        }

        public void SetSearchKeyword(string keyword)
        {
            _searchKeyword = keyword;
            currentPage = 1;
        }

        public void SetMovieFilter(int? movieId)
        {
            _movieFilter = movieId;
            currentPage = 1;
        }

        public void SetActorFilter(int? actorId)
        {
            _actorFilter = actorId;
            currentPage = 1;
        }

        public void SetOrderFilter(int? order)
        {
            _orderFilter = order;
            currentPage = 1;
        }

        public void ClearFilters()
        {
            _searchKeyword = "";
            _movieFilter = null;
            _actorFilter = null;
            _orderFilter = null;
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
        /// Thêm diễn viên vào phim
        /// </summary>
        public bool AddMovieCast(MovieCastDTO cast, out string message)
        {
            // Kiểm tra diễn viên đã tham gia phim chưa
            if (castDAL.HasActorInMovie(cast.MovieID, cast.ActorID))
            {
                message = "Diễn viên đã tham gia phim này rồi";
                return false;
            }

            bool result = castDAL.AddMovieCast(cast);
            message = result ? "Thêm diễn viên vào phim thành công" : "Thêm diễn viên vào phim thất bại";
            return result;
        }

        /// <summary>
        /// Cập nhật thông tin diễn viên trong phim
        /// </summary>
        public bool UpdateMovieCast(MovieCastDTO cast, out string message)
        {
            bool result = castDAL.UpdateMovieCast(cast);
            message = result ? "Cập nhật thông tin thành công" : "Cập nhật thông tin thất bại";
            return result;
        }

        /// <summary>
        /// Xóa diễn viên khỏi phim
        /// </summary>
        public bool DeleteMovieCast(int movieId, int actorId, out string message)
        {
            bool result = castDAL.Delete(movieId, actorId);
            message = result ? "Xóa diễn viên khỏi phim thành công" : "Xóa diễn viên khỏi phim thất bại";
            return result;
        }

        /// <summary>
        /// Xóa nhiều MovieCast
        /// </summary>
        public bool DeleteMultipleMovieCasts(List<(int MovieID, int ActorID)> castIds, out string message)
        {
            int successCount = 0;
            foreach (var (movieId, actorId) in castIds)
            {
                if (castDAL.Delete(movieId, actorId))
                {
                    successCount++;
                }
            }

            message = $"Đã xóa {successCount}/{castIds.Count} diễn viên khỏi phim";
            return successCount > 0;
        }

        /// <summary>
        /// Lấy danh sách actors cho filter/dropdown
        /// </summary>
        public IEnumerable<ActorDTO> GetAllActors()
        {
            return actorDAL.GetAll();
        }

        /// <summary>
        /// Lấy danh sách movies cho filter/dropdown
        /// </summary>
        public IEnumerable<MovieDTO> GetAllMovies()
        {
            return movieDAL.GetAll();
        }

        /// <summary>
        /// Lấy thông tin MovieCast
        /// </summary>
        public MovieCastDTO? GetMovieCast(int movieId, int actorId)
        {
            return castDAL.GetByMovieAndActor(movieId, actorId);
        }

        public List<MovieCastDTO> GetCastByMovie(int movieId, out string message)
        {
            try
            {
                message = "Thành công";
                return castDAL.GetAll().Where(c => c.MovieID == movieId).ToList();
            }
            catch (Exception ex)
            {
                message = ex.Message; return new List<MovieCastDTO>();
            }
        }
    }
}