using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class HistorialParcelasConcesion
{
    public int Id { get; set; }

    public int ConcesionId { get; set; }

    public int ParcelaId { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public int? TramiteOrigenId { get; set; }

    public virtual Concesione Concesion { get; set; } = null!;

    public virtual Parcela Parcela { get; set; } = null!;

    public virtual Tramite? TramiteOrigen { get; set; }
}
