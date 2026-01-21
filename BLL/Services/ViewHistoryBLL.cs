using Muvi.DAL;
using MuVi.DTO.DTOs;
using System.Collections.Generic;

namespace MuVi.BLL
{
    public class ViewHistoryBLL
    {
        private readonly ViewHistoryDAL _historyDAL = new ViewHistoryDAL();

        public List<ViewHistoryDTO> GetUserHistory(int userId)
        {
            if (userId <= 0) return new List<ViewHistoryDTO>();
            return _historyDAL.GetByUserId(userId);
        }

        /// <summary>
        /// Ghi nhận tiến trình xem phim của người dùng
        /// </summary>
        public void SyncProgress(ViewHistoryDTO history)
        {
            // Logic: Nếu xem trên 90% thời lượng phim, coi như đã hoàn thành (IsCompleted = 1)
            // Giả sử bạn có thông tin TotalDuration của phim/tập phim ở đây
            _historyDAL.SaveOrUpdateHistory(history);
        }

        public bool ClearHistoryItem(int historyId, out string message)
        {
            bool result = _historyDAL.DeleteHistory(historyId);
            message = result ? "Đã xóa khỏi lịch sử." : "Xóa thất bại.";
            return result;
        }
    }
}