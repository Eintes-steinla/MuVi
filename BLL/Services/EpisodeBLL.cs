using Muvi.DAL;
using MuVi.DTO.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MuVi.BLL
{
    public class EpisodeBLL
    {
        private EpisodeDAL episodeDAL = new EpisodeDAL();
        private MovieDAL movieDAL = new MovieDAL();

        private int currentPage = 1;
        private int pageSize = 10;

        // Search and filter parameters
        private string _searchKeyword = "";
        private int? _movieFilter = null;
        private int? _yearFilter = null;

        /// <summary>
        /// Lấy danh sách tập phim với filter
        /// </summary>
        public IEnumerable<EpisodeDTO> GetEpisodes()
        {
            var allEpisodes = episodeDAL.GetAll();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(_searchKeyword))
            {
                allEpisodes = allEpisodes.Where(e =>
                    (e.Title?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (e.MovieTitle?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (e.Description?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            // Apply movie filter
            if (_movieFilter != null)
            {
                allEpisodes = allEpisodes.Where(e => e.MovieID == _movieFilter);
            }

            // Apply year filter
            if (_yearFilter != null)
            {
                allEpisodes = allEpisodes.Where(e => e.ReleaseDate.HasValue && e.ReleaseDate.Value.Year == _yearFilter);
            }

            // Apply pagination
            return allEpisodes
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// Lấy tổng số trang
        /// </summary>
        public int GetTotalPages()
        {
            var allEpisodes = episodeDAL.GetAll();

            // Apply same filters
            if (!string.IsNullOrWhiteSpace(_searchKeyword))
            {
                allEpisodes = allEpisodes.Where(e =>
                    (e.Title?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (e.MovieTitle?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (e.Description?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            if (_movieFilter != null)
            {
                allEpisodes = allEpisodes.Where(e => e.MovieID == _movieFilter);
            }

            if (_yearFilter != null)
            {
                allEpisodes = allEpisodes.Where(e => e.ReleaseDate.HasValue && e.ReleaseDate.Value.Year == _yearFilter);
            }

            int totalRecords = allEpisodes.Count();
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

        public void SetYearFilter(int? year)
        {
            _yearFilter = year;
            currentPage = 1;
        }

        public void ClearFilters()
        {
            _searchKeyword = "";
            _movieFilter = null;
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
        /// Thêm tập phim mới
        /// </summary>
        public bool AddEpisode(EpisodeDTO episode, out string message)
        {
            if (episodeDAL.IsEpisodeNumberExists(episode.MovieID, episode.EpisodeNumber))
            {
                message = "Số tập này đã tồn tại trong phim";
                return false;
            }

            bool result = episodeDAL.AddEpisode(episode);
            message = result ? "Thêm tập phim thành công" : "Thêm tập phim thất bại";
            return result;
        }

        /// <summary>
        /// Cập nhật tập phim
        /// </summary>
        public bool UpdateEpisode(EpisodeDTO episode, out string message)
        {
            if (episodeDAL.IsEpisodeNumberExists(episode.MovieID, episode.EpisodeNumber, episode.EpisodeID))
            {
                message = "Số tập này đã tồn tại trong phim";
                return false;
            }

            bool result = episodeDAL.UpdateEpisode(episode);
            message = result ? "Cập nhật tập phim thành công" : "Cập nhật tập phim thất bại";
            return result;
        }

        /// <summary>
        /// Xóa tập phim
        /// </summary>
        public bool DeleteEpisode(int episodeId, out string message)
        {
            bool result = episodeDAL.Delete(episodeId);
            message = result ? "Xóa tập phim thành công" : "Xóa tập phim thất bại";
            return result;
        }

        /// <summary>
        /// Xóa nhiều tập phim
        /// </summary>
        public bool DeleteMultipleEpisodes(List<int> episodeIds, out string message)
        {
            int successCount = 0;
            foreach (int episodeId in episodeIds)
            {
                if (episodeDAL.Delete(episodeId))
                {
                    successCount++;
                }
            }

            message = $"Đã xóa {successCount}/{episodeIds.Count} tập phim";
            return successCount > 0;
        }

        /// <summary>
        /// Lấy danh sách phim (để chọn phim khi thêm/sửa tập)
        /// </summary>
        public IEnumerable<MovieDTO> GetAllMovies()
        {
            return movieDAL.GetAll();
        }

        /// <summary>
        /// Lấy danh sách phim bộ (chỉ phim bộ mới có tập)
        /// </summary>
        public IEnumerable<MovieDTO> GetSeriesMovies()
        {
            return movieDAL.GetAll().Where(m => m.MovieType == "Phim bộ");
        }

        public List<EpisodeDTO> GetEpisodesByMovie(int movieId, out string message)
        {
            try
            {
                message = "Thành công";
                // Lấy toàn bộ từ DAL và lọc theo MovieID
                return episodeDAL.GetAll()
                    .Where(e => e.MovieID == movieId)
                    .OrderBy(e => e.EpisodeNumber)
                    .ToList();
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return new List<EpisodeDTO>();
            }
        }

        public EpisodeDTO? GetEpisodeById(int episodeId) => episodeDAL.GetById(episodeId);
    }
}