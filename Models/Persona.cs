using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class Persona
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    public string? Dni { get; set; }

    public bool Visibilidad { get; set; }

    public DateOnly? FechaNacimiento { get; set; }

    public DateOnly? FechaDefuncion { get; set; }

    public string? InformacionAdicional { get; set; }

    public string? Sexo { get; set; }

    public string? Correo { get; set; }

    public string? Celular { get; set; }

    public string? Domicilio { get; set; }

    public int? NroActa { get; set; }

    public int? NroFolio { get; set; }

    public int? NroTomo { get; set; }

    public string? NroSerie { get; set; }

    public int? NroAge { get; set; }

    public int? EstadoDifuntoId { get; set; }

    public int? CategoriaPersonaId { get; set; }

    public virtual CategoriasPersona? CategoriaPersona { get; set; }

    public virtual ICollection<Cremacione> Cremaciones { get; set; } = new List<Cremacione>();

    public virtual ICollection<DocumentosTramite> DocumentosTramites { get; set; } = new List<DocumentosTramite>();

    public virtual EstadosDifunto? EstadoDifunto { get; set; }

    public virtual ICollection<FirmantesTramite> FirmantesTramites { get; set; } = new List<FirmantesTramite>();

    public virtual ICollection<HistorialTitularesConcesione> HistorialTitularesConcesiones { get; set; } = new List<HistorialTitularesConcesione>();

    public virtual ICollection<Introduccione> Introducciones { get; set; } = new List<Introduccione>();

    public virtual ICollection<ParcelaDifunto> ParcelaDifuntos { get; set; } = new List<ParcelaDifunto>();

    public virtual ICollection<Reduccione> Reducciones { get; set; } = new List<Reduccione>();

    public virtual ICollection<TramitePersona> TramitePersonas { get; set; } = new List<TramitePersona>();

    public virtual ICollection<Traslado> Traslados { get; set; } = new List<Traslado>();
}
