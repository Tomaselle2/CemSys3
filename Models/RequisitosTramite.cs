using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class RequisitosTramite
{
    public int Id { get; set; }

    public int TipoTramiteId { get; set; }

    public string? Descripcion { get; set; }

    public bool? Activo { get; set; }

    public virtual TipoTramite TipoTramite { get; set; } = null!;
}
