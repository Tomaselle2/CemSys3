using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class Traslado
{
    public int TramiteId { get; set; }

    public int ParcelaOrigenId { get; set; }

    public int? ParcelaDestinoId { get; set; }

    public int UsuarioId { get; set; }

    public int DifuntoId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaPendiente { get; set; }

    public DateTime? FechaFinalizacion { get; set; }

    public string? Destino { get; set; }

    public string? InfoAdicional { get; set; }

    public int ConcesionId { get; set; }

    public int? CementerioId { get; set; }

    public bool? Visibilidad { get; set; }

    public int? TipoTraslado { get; set; }

    public virtual Cementerio? Cementerio { get; set; }

    public virtual Tramite Concesion { get; set; } = null!;

    public virtual Persona Difunto { get; set; } = null!;

    public virtual Parcela? ParcelaDestino { get; set; }

    public virtual Parcela ParcelaOrigen { get; set; } = null!;

    public virtual Tramite Tramite { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
