namespace MuVi.DTO.DTOs
{
    public class ReviewDTO
    {
        public int ReviewID { get; set; }
        public int MovieID { get; set; }
        public int UserID { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public int LikeCount { get; set; }
    }
}
