using Muvi.DAL;
using MuVi.DTO.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MuVi.BLL
{
    public class ViewHistoryBLL
    {
        private ViewHistoryDAL historyDAL = new ViewHistoryDAL();
        private UserDAL userDAL = new UserDAL();

        private int currentPage = 1;
        private int pageSize = 10;

        // Filter parameters
        private string _searchKeyword = "";
        private string _statusFilter = "Tất cả";  // Tất cả, Đã xem hết, Chưa xem hết
        private string _movieTypeFilter = "Tất cả";  // Tất cả, Phim lẻ, Phim bộ
        private int? _userFilter = null;
        private DateTime? _fromDate = null;
        private DateTime? _toDate = null;

        /// <summary>
        /// Lấy danh sách lịch sử với filter và phân trang
        /// </summary>
        public IEnumerable<ViewHistoryDTO> GetHistories()
        {
            var allHistories = historyDAL.GetAll();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(_searchKeyword))
            {
                allHistories = allHistories.Where(h =>
                    (h.Username?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (h.MovieTitle?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (h.EpisodeTitle?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            // Apply status filter (completed/incomplete)
            if (_statusFilter == "Đã xem hết")
            {
                allHistories = allHistories.Where(h => h.IsCompleted);
            }
            else if (_statusFilter == "Chưa xem hết")
            {
                allHistories = allHistories.Where(h => !h.IsCompleted);
            }

            // Apply movie type filter
            if (_movieTypeFilter != "Tất cả")
            {
                allHistories = allHistories.Where(h => h.MovieType == _movieTypeFilter);
            }

            // Apply user filter
            if (_userFilter.HasValue)
            {
                allHistories = allHistories.Where(h => h.UserID == _userFilter.Value);
            }

            // Apply date range filter
            if (_fromDate.HasValue)
            {
                allHistories = allHistories.Where(h => h.WatchedAt >= _fromDate.Value);
            }

            if (_toDate.HasValue)
            {
                allHistories = allHistories.Where(h => h.WatchedAt <= _toDate.Value);
            }

            // Apply pagination
            return allHistories
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// Lấy tổng số trang
        /// </summary>
        public int GetTotalPages()
        {
            var allHistories = historyDAL.GetAll();

            // Apply same filters
            if (!string.IsNullOrWhiteSpace(_searchKeyword))
            {
                allHistories = allHistories.Where(h =>
                    (h.Username?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (h.MovieTitle?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (h.EpisodeTitle?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            if (_statusFilter == "Đã xem hết")
            {
                allHistories = allHistories.Where(h => h.IsCompleted);
            }
            else if (_statusFilter == "Chưa xem hết")
            {
                allHistories = allHistories.Where(h => !h.IsCompleted);
            }

            if (_movieTypeFilter != "Tất cả")
            {
                allHistories = allHistories.Where(h => h.MovieType == _movieTypeFilter);
            }

            if (_userFilter.HasValue)
            {
                allHistories = allHistories.Where(h => h.UserID == _userFilter.Value);
            }

            if (_fromDate.HasValue)
            {
                allHistories = allHistories.Where(h => h.WatchedAt >= _fromDate.Value);
            }

            if (_toDate.HasValue)
            {
                allHistories = allHistories.Where(h => h.WatchedAt <= _toDate.Value);
            }

            int totalRecords = allHistories.Count();
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

        public void SetUserFilter(int? userId)
        {
            _userFilter = userId;
            currentPage = 1;
        }

        public void SetDateRange(DateTime? fromDate, DateTime? toDate)
        {
            _fromDate = fromDate;
            _toDate = toDate;
            currentPage = 1;
        }

        public void ClearFilters()
        {
            _searchKeyword = "";
            _statusFilter = "Tất cả";
            _movieTypeFilter = "Tất cả";
            _userFilter = null;
            _fromDate = null;
            _toDate = null;
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
        /// Thêm hoặc cập nhật lịch sử xem
        /// </summary>
        public bool AddOrUpdateHistory(ViewHistoryDTO history, out string message)
        {
            bool result = historyDAL.AddOrUpdateHistory(history);
            message = result ? "Cập nhật lịch sử xem thành công" : "Cập nhật lịch sử xem thất bại";
            return result;
        }

        /// <summary>
        /// Xóa lịch sử
        /// </summary>
        public bool DeleteHistory(int historyId, out string message)
        {
            bool result = historyDAL.Delete(historyId);
            message = result ? "Xóa lịch sử thành công" : "Xóa lịch sử thất bại";
            return result;
        }

        /// <summary>
        /// Xóa nhiều lịch sử
        /// </summary>
        public bool DeleteMultipleHistories(List<int> historyIds, out string message)
        {
            int successCount = 0;
            foreach (int historyId in historyIds)
            {
                if (historyDAL.Delete(historyId))
                {
                    successCount++;
                }
            }

            message = $"Đã xóa {successCount}/{historyIds.Count} lịch sử";
            return successCount > 0;
        }

        /// <summary>
        /// Lấy danh sách users cho filter
        /// </summary>
        public IEnumerable<UserDTO> GetAllUsers()
        {
            return userDAL.GetAll();
        }

        /// <summary>
        /// Lấy thông tin lịch sử theo ID
        /// </summary>
        public ViewHistoryDTO? GetHistoryById(int historyId)
        {
            return historyDAL.GetById(historyId);
        }
    }
}