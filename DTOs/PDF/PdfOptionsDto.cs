namespace CemSys3.DTOs.PDF
{
    public class PdfOptionsDto
    {
        public bool Landscape { get; set; } = false;
        public string Format { get; set; } = "A4";
        public string MarginTop { get; set; } = "20px";
        public string MarginBottom { get; set; } = "20px";
        public string MarginLeft { get; set; } = "20px";
        public string MarginRight { get; set; } = "20px";
    }
}
