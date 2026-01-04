using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class AnioConcesion
{
    public int Id { get; set; }

    public int Anios { get; set; }

    public virtual ICollection<Concesione> Concesiones { get; set; } = new List<Concesione>();
}
