namespace CemSys3.DTOs.Calendario
{
    public class CalendarDTO
    {
        public int id { get; set; }
        public DateTime start { get; set; }
        public string title { get; set; } = string.Empty;

        public string? color { get; set; }
        public string? url { get; set; }
        public int tramiteId { get; set; }
        public string? tipo { get; set; }
        public bool allDay { get; set; }
    }
}
