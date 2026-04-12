using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class DocumentosTramite
{
    public int Id { get; set; }

    public int TramiteId { get; set; }

    public int? PlantillaId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? ContenidoHtml { get; set; }

    public int Version { get; set; }

    public DateTime? FechaUltimaEdicion { get; set; }

    public int UsuarioId { get; set; }

    public bool? Visibilidad { get; set; }

    public int? PersonaId { get; set; }

    public string? Parentesco { get; set; }

    public int? TipoAutorizacionId { get; set; }

    public virtual Persona? Persona { get; set; }

    public virtual PlantillasTramite? Plantilla { get; set; }

    public virtual TipoAutorizacion? TipoAutorizacion { get; set; }

    public virtual Tramite Tramite { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
