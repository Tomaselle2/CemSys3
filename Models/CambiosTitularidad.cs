using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class CambiosTitularidad
{
    public int TramiteId { get; set; }

    public int ParcelaId { get; set; }

    public int UsuarioId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaFinalizacion { get; set; }

    public string? InfoAdicional { get; set; }

    public int? ConcesionId { get; set; }

    public bool? Visibilidad { get; set; }

    public virtual Tramite? Concesion { get; set; }

    public virtual Parcela Parcela { get; set; } = null!;

    public virtual Tramite Tramite { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
