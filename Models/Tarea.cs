using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class Tarea
{
    public int Id { get; set; }

    public bool Estado { get; set; }

    public string Descripcion { get; set; } = null!;

    public int? NotaId { get; set; }

    public int? TramiteId { get; set; }

    public virtual Nota? Nota { get; set; }

    public virtual Tramite? Tramite { get; set; }
}
