using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class EstadosDifunto
{
    public int Id { get; set; }

    public string Estado { get; set; } = null!;

    public virtual ICollection<Persona> Personas { get; set; } = new List<Persona>();
}
