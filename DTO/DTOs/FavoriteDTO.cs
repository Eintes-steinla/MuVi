namespace MuVi.DTO.DTOs
{
    public class FavoriteDTO
    {
        public int UserID { get; set; }
        public int MovieID { get; set; }
        public string AddedAt { get; set; } = "";

        // Các thuộc tính mở rộng để hiển thị lên giao diện
        public string Title { get; set; } = "";
        public string PosterPath { get; set; } = "";
        public int ReleaseYear { get; set; }
    }
}