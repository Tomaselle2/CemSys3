using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class Cementerio
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Visibilidad { get; set; }
}
