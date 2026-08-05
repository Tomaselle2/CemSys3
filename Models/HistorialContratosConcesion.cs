using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class HistorialContratosConcesion
{
    public int Id { get; set; }

    public int TramiteId { get; set; }

    public int? Concesion { get; set; }

    public int ParcelaId { get; set; }

    public DateTime FechaContrato { get; set; }

    public bool EsRenovacion { get; set; }

    public int? UsuarioId { get; set; }

    public bool Visibilidad { get; set; }

    public virtual ICollection<HistorialContratosConcesionDifunto> HistorialContratosConcesionDifuntos { get; set; } = new List<HistorialContratosConcesionDifunto>();

    public virtual Parcela Parcela { get; set; } = null!;

    public virtual Tramite Tramite { get; set; } = null!;

    public virtual Usuario? Usuario { get; set; }
}
