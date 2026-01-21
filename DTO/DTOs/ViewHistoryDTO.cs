namespace MuVi.DTO.DTOs
{
    public class ViewHistoryDTO
    {
        public int HistoryID { get; set; }
        public int UserID { get; set; }
        public int MovieID { get; set; }
        public int EpisodeID { get; set; }
        public string WatchedAt { get; set; } = "";
        public int WatchDuration { get; set; }
        public bool IsCompleted { get; set; }
    }
}
