namespace MuVi.DTO.DTOs
{
    public class ViewHistoryDTO
    {
        public int HistoryID { get; set; }
        public int UserID { get; set; }
        public int MovieID { get; set; }
        public int? EpisodeID { get; set; } // Cho phép null nếu là phim lẻ
        public string WatchedAt { get; set; } = "";
        public int WatchDuration { get; set; }
        public bool IsCompleted { get; set; }

        // Thuộc tính bổ sung để hiển thị trên UI
        public string? MovieTitle { get; set; }
        public string? PosterPath { get; set; }
        public int? EpisodeNumber { get; set; } // Số tập nếu là phim bộ
    }
}