using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class FirmantesTramite
{
    public int Id { get; set; }

    public int TramiteId { get; set; }

    public int PersonaId { get; set; }

    public string? Parentesco { get; set; }

    public bool EsTitular { get; set; }

    public DateTime? FechaAlta { get; set; }

    public bool? Visibilidad { get; set; }

    public virtual ICollection<DocumentosTramite> DocumentosTramites { get; set; } = new List<DocumentosTramite>();

    public virtual Persona Persona { get; set; } = null!;

    public virtual Tramite Tramite { get; set; } = null!;
}
