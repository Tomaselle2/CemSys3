using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class TemasTarifarium
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Visibilidad { get; set; }

    public virtual ICollection<ConceptosTarifarium> ConceptosTarifaria { get; set; } = new List<ConceptosTarifarium>();
}
