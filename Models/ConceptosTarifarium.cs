using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class ConceptosTarifarium
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Visibilidad { get; set; }

    public int TemaId { get; set; }

    public virtual ICollection<PreciosTarifaria> PreciosTarifaria { get; set; } = new List<PreciosTarifaria>();

    public virtual ICollection<ReglasIngreso> ReglasIngresoCierreFosaNavigations { get; set; } = new List<ReglasIngreso>();

    public virtual ICollection<ReglasIngreso> ReglasIngresoCierreNichoNavigations { get; set; } = new List<ReglasIngreso>();

    public virtual ICollection<ReglasIngreso> ReglasIngresoConceptoDefuncions { get; set; } = new List<ReglasIngreso>();

    public virtual ICollection<ReglasIngreso> ReglasIngresoConceptoInhumacions { get; set; } = new List<ReglasIngreso>();

    public virtual ICollection<ReglasIngreso> ReglasIngresoConceptoIntroduccions { get; set; } = new List<ReglasIngreso>();

    public virtual ICollection<ReglasIngreso> ReglasIngresoConceptoTranscripcions { get; set; } = new List<ReglasIngreso>();

    public virtual ICollection<ReglasIngreso> ReglasIngresoMontoMinimoFondos { get; set; } = new List<ReglasIngreso>();

    public virtual ICollection<ReglasIngreso> ReglasIngresoPorcentajeAumentoDerechoOficinas { get; set; } = new List<ReglasIngreso>();

    public virtual ICollection<ReglasIngreso> ReglasIngresoPorcentajeAumentoOtraLocalidads { get; set; } = new List<ReglasIngreso>();

    public virtual ICollection<ReglasIngreso> ReglasIngresoPorcentajeFondoSaluds { get; set; } = new List<ReglasIngreso>();

    public virtual ICollection<ReglasIngreso> ReglasIngresoPorcentajeIntroduccionUrnaDerechoOficnaNavigations { get; set; } = new List<ReglasIngreso>();

    public virtual TemasTarifarium Tema { get; set; } = null!;

    public virtual ICollection<TramitesCosto> TramitesCostos { get; set; } = new List<TramitesCosto>();
}
