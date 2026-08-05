using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class ControlProcesosDiario
{
    public int Id { get; set; }

    public string NombreProceso { get; set; } = null!;

    public DateOnly FechaEjecucion { get; set; }

    public DateTime FechaHoraEjecucion { get; set; }

    public int? CantidadActualizadas { get; set; }
}
