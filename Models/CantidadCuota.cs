using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class CantidadCuota
{
    public int Id { get; set; }

    public int Cuota { get; set; }

    public virtual ICollection<Concesione> Concesiones { get; set; } = new List<Concesione>();
}
