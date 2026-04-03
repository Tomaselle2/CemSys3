using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class TramitesCosto
{
    public int Id { get; set; }

    public int TramiteId { get; set; }

    public int ConceptoTarifariaId { get; set; }

    public decimal Monto { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public bool? Visibilidad { get; set; }

    public virtual ConceptosTarifarium ConceptoTarifaria { get; set; } = null!;

    public virtual Tramite Tramite { get; set; } = null!;
}
