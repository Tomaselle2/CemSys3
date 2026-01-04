using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class Concesione
{
    public int TramiteId { get; set; }

    public string? Concesion { get; set; }

    public string? PagoDescripcion { get; set; }

    public DateTime? FechaGeneracion { get; set; }

    public decimal? Precio { get; set; }

    public bool? Visibilidad { get; set; }

    public string? TipoParcela { get; set; }

    public DateOnly? Vencimiento { get; set; }

    public int ParcelaId { get; set; }

    public int? CantidadAniosId { get; set; }

    public int? CuotaId { get; set; }

    public int? UsuarioId { get; set; }

    public virtual AnioConcesion? CantidadAnios { get; set; }

    public virtual CantidadCuota? Cuota { get; set; }

    public virtual ICollection<HistorialTitularesConcesione> HistorialTitularesConcesiones { get; set; } = new List<HistorialTitularesConcesione>();

    public virtual Parcela Parcela { get; set; } = null!;

    public virtual Tramite Tramite { get; set; } = null!;

    public virtual Usuario? Usuario { get; set; }
}
