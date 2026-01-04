using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class Archivo
{
    public Guid Id { get; set; }

    public string? CategoriaArchivo { get; set; }

    public int? TramiteId { get; set; }

    public string NombreArchivo { get; set; } = null!;

    public string TipoArchivo { get; set; } = null!;

    public long TamanoBytes { get; set; }

    public byte[] Contenido { get; set; } = null!;

    public string? Descripcion { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public bool Visibilidad { get; set; }

    public virtual Tramite? Tramite { get; set; }
}
