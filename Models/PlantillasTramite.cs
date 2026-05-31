using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class PlantillasTramite
{
    public int Id { get; set; }

    public int TipoTramiteId { get; set; }

    public string? Nombre { get; set; }

    public string? Contenido { get; set; }

    public int? TipoAutorizacionId { get; set; }

    public bool? Activo { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public virtual ICollection<DocumentosTramite> DocumentosTramites { get; set; } = new List<DocumentosTramite>();

    public virtual TipoAutorizacion? TipoAutorizacion { get; set; }

    public virtual TipoTramite TipoTramite { get; set; } = null!;
}
