using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class Parcela
{
    public int Id { get; set; }

    public bool Visibilidad { get; set; }

    public int NroParcela { get; set; }

    public int NroFila { get; set; }

    public int CantidadDifuntos { get; set; }

    public string NombrePanteon { get; set; } = null!;

    public string InformacionAdicional { get; set; } = null!;

    public int SeccionId { get; set; }

    public int? TipoNichoId { get; set; }

    public int? TipoPanteonId { get; set; }

    public int? TipoParcelaId { get; set; }

    public virtual ICollection<AceptacionTitularidad> AceptacionTitularidads { get; set; } = new List<AceptacionTitularidad>();

    public virtual ICollection<CambiosTitularidad> CambiosTitularidads { get; set; } = new List<CambiosTitularidad>();

    public virtual ICollection<Concesione> Concesiones { get; set; } = new List<Concesione>();

    public virtual ICollection<Cremacione> CremacioneParcelaDestinos { get; set; } = new List<Cremacione>();

    public virtual ICollection<Cremacione> CremacioneParcelaOrigens { get; set; } = new List<Cremacione>();

    public virtual ICollection<Introduccione> Introducciones { get; set; } = new List<Introduccione>();

    public virtual ICollection<ParcelaDifunto> ParcelaDifuntos { get; set; } = new List<ParcelaDifunto>();

    public virtual ICollection<Reduccione> ReduccioneParcelaDestinos { get; set; } = new List<Reduccione>();

    public virtual ICollection<Reduccione> ReduccioneParcelaOrigens { get; set; } = new List<Reduccione>();

    public virtual Seccione Seccion { get; set; } = null!;

    public virtual TipoNicho? TipoNicho { get; set; }

    public virtual TipoPanteon? TipoPanteon { get; set; }

    public virtual TipoParcela? TipoParcela { get; set; }

    public virtual ICollection<TramitesParcela> TramitesParcelas { get; set; } = new List<TramitesParcela>();

    public virtual ICollection<Traslado> TrasladoParcelaDestinos { get; set; } = new List<Traslado>();

    public virtual ICollection<Traslado> TrasladoParcelaOrigens { get; set; } = new List<Traslado>();
}
