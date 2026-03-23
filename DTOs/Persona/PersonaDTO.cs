namespace CemSys3.DTOs.Persona
{
    public class PersonaDTO
    {
        public int Id { get; set; }

        public string? Nombre { get; set; }

        public string? Apellido { get; set; }

        public string? Dni { get; set; }

        public bool Visibilidad { get; set; }

        public DateOnly? FechaNacimiento { get; set; }

        public DateOnly? FechaDefuncion { get; set; }

        public string? InformacionAdicional { get; set; }

        public string? Sexo { get; set; }

        public string? Correo { get; set; }

        public string? Celular { get; set; }

        public string? Domicilio { get; set; }

        public int? NroActa { get; set; }

        public int? NroFolio { get; set; }

        public int? NroTomo { get; set; }

        public string? NroSerie { get; set; }

        public int? NroAge { get; set; }

        public int? EstadoDifuntoId { get; set; }

        public int? CategoriaPersonaId { get; set; }

        public DateTime? FechaIngreso { get; set; }
    }
}
