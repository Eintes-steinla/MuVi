namespace MuVi.DTO.DTOs
{
    public class EpisodeDTO
    {
        public int EpisodeID { get; set; }
        public int MovieID { get; set; }
        public int EpisodeNumber { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public int Duration { get; set; }
        public string? VideoPath { get; set; }
        public string? ReleaseDate { get; set; }
        public int ViewCount { get; set; }
    }
}
