using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class Diagrama
{
    public int Id { get; set; }

    public int TramiteId { get; set; }

    public string? JsonDiagrama { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }
}
