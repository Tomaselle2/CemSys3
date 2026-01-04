using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class TipoNotum
{
    public int Id { get; set; }

    public string Descripcion { get; set; } = null!;

    public bool Visibilidad { get; set; }

    public virtual ICollection<Nota> Nota { get; set; } = new List<Nota>();
}
