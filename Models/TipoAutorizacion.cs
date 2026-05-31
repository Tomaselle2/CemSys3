using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class TipoAutorizacion
{
    public int Id { get; set; }

    public int TipoTramiteId { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<DocumentosTramite> DocumentosTramites { get; set; } = new List<DocumentosTramite>();

    public virtual ICollection<PlantillasTramite> PlantillasTramites { get; set; } = new List<PlantillasTramite>();

    public virtual TipoTramite TipoTramite { get; set; } = null!;
}
