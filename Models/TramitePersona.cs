using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class TramitePersona
{
    public int TramiteId { get; set; }

    public int PersonaId { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual Persona Persona { get; set; } = null!;

    public virtual Tramite Tramite { get; set; } = null!;
}
