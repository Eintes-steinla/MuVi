namespace MuVi.DTO.DTOs
{
    public class ActorDTO
    {
        public int ActorID { get; set; }
        public string ActorName { get; set; } = "";
        public string? Bio { get; set; }
        public string? PhotoPath { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Nationality { get; set; }
    }
}
