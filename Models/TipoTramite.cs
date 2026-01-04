using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class TipoTramite
{
    public int Id { get; set; }

    public string Tipo { get; set; } = null!;

    public bool Visibilidad { get; set; }

    public virtual ICollection<EstadosTramite> EstadosTramites { get; set; } = new List<EstadosTramite>();

    public virtual ICollection<Tramite> Tramites { get; set; } = new List<Tramite>();
}
