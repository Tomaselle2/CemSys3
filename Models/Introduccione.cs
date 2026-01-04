using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class Introduccione
{
    public int TramiteId { get; set; }

    public bool Visibilidad { get; set; }

    public DateTime? FechaIngreso { get; set; }

    public int UsuarioId { get; set; }

    public int? EmpresaFunebreId { get; set; }

    public int ParcelaId { get; set; }

    public int DifuntoId { get; set; }

    public string? EstadoDifunto { get; set; }

    public string? InformacionAdicional { get; set; }

    public virtual Persona Difunto { get; set; } = null!;

    public virtual EmpresasFunebre? EmpresaFunebre { get; set; }

    public virtual Parcela Parcela { get; set; } = null!;

    public virtual Tramite Tramite { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
