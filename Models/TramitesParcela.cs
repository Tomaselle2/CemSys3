using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class TramitesParcela
{
    public int TramiteId { get; set; }

    public int ParcelaId { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual Parcela Parcela { get; set; } = null!;

    public virtual Tramite Tramite { get; set; } = null!;
}
