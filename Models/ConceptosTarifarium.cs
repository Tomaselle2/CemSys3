using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class ConceptosTarifarium
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Visibilidad { get; set; }

    public int TemaId { get; set; }

    public virtual ICollection<PreciosTarifaria> PreciosTarifaria { get; set; } = new List<PreciosTarifaria>();

    public virtual TemasTarifarium Tema { get; set; } = null!;
}
