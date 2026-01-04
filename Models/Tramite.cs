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

    public virtual ICollection<Archivo> Archivos { get; set; } = new List<Archivo>();

    public virtual Concesione? Concesione { get; set; }

    public virtual EstadosTramite EstadoActual { get; set; } = null!;

    public virtual ICollection<HistorialEstadoTramite> HistorialEstadoTramites { get; set; } = new List<HistorialEstadoTramite>();

    public virtual Introduccione? Introduccione { get; set; }

    public virtual ICollection<ParcelaDifunto> ParcelaDifuntoTramiteIngresos { get; set; } = new List<ParcelaDifunto>();

    public virtual ICollection<ParcelaDifunto> ParcelaDifuntoTramiteRetiros { get; set; } = new List<ParcelaDifunto>();

    public virtual ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();

    public virtual TipoTramite TipoTramite { get; set; } = null!;

    public virtual ICollection<TramitePersona> TramitePersonas { get; set; } = new List<TramitePersona>();

    public virtual ICollection<TramitesParcela> TramitesParcelas { get; set; } = new List<TramitesParcela>();

    public virtual Usuario Usuario { get; set; } = null!;
}
