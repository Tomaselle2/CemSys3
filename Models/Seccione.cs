using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class Seccione
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Visibilidad { get; set; }

    public int Filas { get; set; }

    public int NroParcelas { get; set; }

    public int TipoNumeracionParcelaId { get; set; }

    public int TipoParcelaId { get; set; }

    public virtual ICollection<Parcela> Parcelas { get; set; } = new List<Parcela>();

    public virtual ICollection<PreciosTarifaria> PreciosTarifaria { get; set; } = new List<PreciosTarifaria>();

    public virtual TipoNumeracionParcela TipoNumeracionParcela { get; set; } = null!;

    public virtual TipoParcela TipoParcela { get; set; } = null!;
}
