using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class PlantillasTramite
{
    public int Id { get; set; }

    public int TipoTramiteId { get; set; }

    public string? Nombre { get; set; }

    public string? Contenido { get; set; }

    public int? TipoEscenario { get; set; }

    public bool? Activo { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public virtual TipoTramite TipoTramite { get; set; } = null!;
}
