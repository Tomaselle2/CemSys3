using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class HistorialTitularesConcesione
{
    public int Id { get; set; }

    public int? ConcesionId { get; set; }

    public int? PersonaId { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public virtual Concesione? Concesion { get; set; }

    public virtual Persona? Persona { get; set; }
}
