using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class ReglasIngreso
{
    public int Id { get; set; }

    public int TipoParcelaId { get; set; }

    public int EstadoDifuntoId { get; set; }

    public int? TipoNichoId { get; set; }

    public int? TipoPanteonId { get; set; }

    public int ConceptoInhumacionId { get; set; }

    public int ConceptoDefuncionId { get; set; }

    public int? ConceptoTranscripcionId { get; set; }

    public int ConceptoIntroduccionId { get; set; }

    public int PorcentajeFondoSaludId { get; set; }

    public int? PorcentajeAumentoOtraLocalidadId { get; set; }

    public int? PorcentajeAumentoDerechoOficinaId { get; set; }

    public int? PorcentajeIntroduccionUrnaDerechoOficna { get; set; }

    public int? MontoMinimoFondoId { get; set; }

    public bool Visibilidad { get; set; }

    public string NombreRegla { get; set; } = null!;

    public int? CierreNicho { get; set; }

    public int? CierreFosa { get; set; }

    public virtual ConceptosTarifarium? CierreFosaNavigation { get; set; }

    public virtual ConceptosTarifarium? CierreNichoNavigation { get; set; }

    public virtual ConceptosTarifarium ConceptoDefuncion { get; set; } = null!;

    public virtual ConceptosTarifarium ConceptoInhumacion { get; set; } = null!;

    public virtual ConceptosTarifarium ConceptoIntroduccion { get; set; } = null!;

    public virtual ConceptosTarifarium? ConceptoTranscripcion { get; set; }

    public virtual EstadosDifunto EstadoDifunto { get; set; } = null!;

    public virtual ConceptosTarifarium? MontoMinimoFondo { get; set; }

    public virtual ConceptosTarifarium? PorcentajeAumentoDerechoOficina { get; set; }

    public virtual ConceptosTarifarium? PorcentajeAumentoOtraLocalidad { get; set; }

    public virtual ConceptosTarifarium PorcentajeFondoSalud { get; set; } = null!;

    public virtual ConceptosTarifarium? PorcentajeIntroduccionUrnaDerechoOficnaNavigation { get; set; }

    public virtual TipoNicho? TipoNicho { get; set; }

    public virtual TipoPanteon? TipoPanteon { get; set; }

    public virtual TipoParcela TipoParcela { get; set; } = null!;
}
