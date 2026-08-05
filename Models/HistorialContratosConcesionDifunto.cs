using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class HistorialContratosConcesionDifunto
{
    public int Id { get; set; }

    public int HistorialContratoId { get; set; }

    public int DifuntoId { get; set; }

    public virtual Persona Difunto { get; set; } = null!;

    public virtual HistorialContratosConcesion HistorialContrato { get; set; } = null!;
}
