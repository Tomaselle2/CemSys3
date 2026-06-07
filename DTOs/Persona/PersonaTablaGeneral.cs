namespace CemSys3.DTOs.Persona
{
    public class PersonaTablaGeneral
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Apellido { get; set; } = string.Empty;

        public string? Dni { get; set; }

        public bool Visibilidad { get; set; }

        public int CategoriaPersonaId { get; set; }

        public string celular { get; set; } = string.Empty;
        public string correo { get; set; } = string.Empty;

    }
}
