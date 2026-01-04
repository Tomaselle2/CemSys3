using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class TipoParcela
{
    public int Id { get; set; }

    public string Tipo { get; set; } = null!;

    public virtual ICollection<Parcela> Parcelas { get; set; } = new List<Parcela>();

    public virtual ICollection<Seccione> Secciones { get; set; } = new List<Seccione>();
}
