using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class EmpresasFunebre
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Visibilidad { get; set; }

    public virtual ICollection<Introduccione> Introducciones { get; set; } = new List<Introduccione>();
}
