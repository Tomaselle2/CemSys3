using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class PreciosTarifaria
{
    public int Id { get; set; }

    public decimal Precio { get; set; }

    public int? NroFila { get; set; }

    public int ConceptoTarifariaId { get; set; }

    public int? AniosConcesionId { get; set; }

    public int? SeccionId { get; set; }

    public virtual ConceptosTarifarium ConceptoTarifaria { get; set; } = null!;

    public virtual Seccione? Seccion { get; set; }
}
