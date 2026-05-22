using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class Tramite
{
    public int Id { get; set; }

    public bool Visibilidad { get; set; }

    public DateTime FechaCreacion { get; set; }

    public int TipoTramiteId { get; set; }

    public int UsuarioId { get; set; }

    public int EstadoActualId { get; set; }

    public DateTime? FechaFinalizacion { get; set; }

    public virtual ICollection<AceptacionTitularidad> AceptacionTitularidadConcesions { get; set; } = new List<AceptacionTitularidad>();

    public virtual AceptacionTitularidad? AceptacionTitularidadTramite { get; set; }

    public virtual ICollection<Archivo> Archivos { get; set; } = new List<Archivo>();

    public virtual ICollection<CambiosTitularidad> CambiosTitularidadConcesions { get; set; } = new List<CambiosTitularidad>();

    public virtual CambiosTitularidad? CambiosTitularidadTramite { get; set; }

    public virtual Concesione? Concesione { get; set; }

    public virtual ICollection<Cremacione> CremacioneConcesions { get; set; } = new List<Cremacione>();

    public virtual Cremacione? CremacioneTramite { get; set; }

    public virtual ICollection<DocumentosTramite> DocumentosTramites { get; set; } = new List<DocumentosTramite>();

    public virtual EstadosTramite EstadoActual { get; set; } = null!;

    public virtual ICollection<FirmantesTramite> FirmantesTramites { get; set; } = new List<FirmantesTramite>();

    public virtual ICollection<HistorialEstadoTramite> HistorialEstadoTramites { get; set; } = new List<HistorialEstadoTramite>();

    public virtual Introduccione? Introduccione { get; set; }

    public virtual Nota? NotaTramite { get; set; }

    public virtual ICollection<Nota> NotaTramiteIngresos { get; set; } = new List<Nota>();

    public virtual ICollection<ParcelaDifunto> ParcelaDifuntoTramiteIngresos { get; set; } = new List<ParcelaDifunto>();

    public virtual ICollection<ParcelaDifunto> ParcelaDifuntoTramiteRetiros { get; set; } = new List<ParcelaDifunto>();

    public virtual ICollection<Reduccione> ReduccioneConcesions { get; set; } = new List<Reduccione>();

    public virtual Reduccione? ReduccioneTramite { get; set; }

    public virtual ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();

    public virtual TipoTramite TipoTramite { get; set; } = null!;

    public virtual ICollection<TramitePersona> TramitePersonas { get; set; } = new List<TramitePersona>();

    public virtual ICollection<TramitesCosto> TramitesCostos { get; set; } = new List<TramitesCosto>();

    public virtual ICollection<TramitesParcela> TramitesParcelas { get; set; } = new List<TramitesParcela>();

    public virtual ICollection<Traslado> TrasladoConcesions { get; set; } = new List<Traslado>();

    public virtual Traslado? TrasladoTramite { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
