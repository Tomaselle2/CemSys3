using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class Cementerio
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Visibilidad { get; set; }

    public virtual ICollection<Cremacione> Cremaciones { get; set; } = new List<Cremacione>();

    public virtual ICollection<Reduccione> Reducciones { get; set; } = new List<Reduccione>();

    public virtual ICollection<Traslado> Traslados { get; set; } = new List<Traslado>();
}
