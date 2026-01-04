using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class TipoNumeracionParcela
{
    public int Id { get; set; }

    public string TipoNumeracion { get; set; } = null!;

    public virtual ICollection<Seccione> Secciones { get; set; } = new List<Seccione>();
}
