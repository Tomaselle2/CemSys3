using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class EstadosTramite
{
    public int Id { get; set; }

    public string Estado { get; set; } = null!;

    public bool Visibilidad { get; set; }

    public int TipoTramiteId { get; set; }

    public virtual ICollection<HistorialEstadoTramite> HistorialEstadoTramites { get; set; } = new List<HistorialEstadoTramite>();

    public virtual TipoTramite TipoTramite { get; set; } = null!;

    public virtual ICollection<Tramite> Tramites { get; set; } = new List<Tramite>();
}
