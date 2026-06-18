using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AceptacionTitularidad> AceptacionTitularidads { get; set; }

    public virtual DbSet<AnioConcesion> AnioConcesions { get; set; }

    public virtual DbSet<Archivo> Archivos { get; set; }

    public virtual DbSet<CambiosTitularidad> CambiosTitularidads { get; set; }

    public virtual DbSet<CantidadCuota> CantidadCuotas { get; set; }

    public virtual DbSet<CategoriasPersona> CategoriasPersonas { get; set; }

    public virtual DbSet<Cementerio> Cementerios { get; set; }

    public virtual DbSet<ConceptosTarifarium> ConceptosTarifaria { get; set; }

    public virtual DbSet<Concesione> Concesiones { get; set; }

    public virtual DbSet<Cremacione> Cremaciones { get; set; }

    public virtual DbSet<Diagrama> Diagramas { get; set; }

    public virtual DbSet<DocumentosTramite> DocumentosTramites { get; set; }

    public virtual DbSet<EmpresasFunebre> EmpresasFunebres { get; set; }

    public virtual DbSet<EstadosDifunto> EstadosDifuntos { get; set; }

    public virtual DbSet<EstadosTramite> EstadosTramites { get; set; }

    public virtual DbSet<EventoCalendario> EventoCalendarios { get; set; }

    public virtual DbSet<FirmantesTramite> FirmantesTramites { get; set; }

    public virtual DbSet<HistorialEstadoTramite> HistorialEstadoTramites { get; set; }

    public virtual DbSet<HistorialParcelasConcesion> HistorialParcelasConcesions { get; set; }

    public virtual DbSet<HistorialTitularesConcesione> HistorialTitularesConcesiones { get; set; }

    public virtual DbSet<Introduccione> Introducciones { get; set; }

    public virtual DbSet<Nota> Notas { get; set; }

    public virtual DbSet<Parcela> Parcelas { get; set; }

    public virtual DbSet<ParcelaDifunto> ParcelaDifuntos { get; set; }

    public virtual DbSet<PermisosIngreso> PermisosIngresos { get; set; }

    public virtual DbSet<PermisosRefaccione> PermisosRefacciones { get; set; }

    public virtual DbSet<Persona> Personas { get; set; }

    public virtual DbSet<PlantillasTramite> PlantillasTramites { get; set; }

    public virtual DbSet<PreciosTarifaria> PreciosTarifarias { get; set; }

    public virtual DbSet<Reduccione> Reducciones { get; set; }

    public virtual DbSet<ReglasIngreso> ReglasIngresos { get; set; }

    public virtual DbSet<RequisitosTramite> RequisitosTramites { get; set; }

    public virtual DbSet<RolesUsuario> RolesUsuarios { get; set; }

    public virtual DbSet<Seccione> Secciones { get; set; }

    public virtual DbSet<Tarea> Tareas { get; set; }

    public virtual DbSet<TareaPlantilla> TareaPlantillas { get; set; }

    public virtual DbSet<TemasTarifarium> TemasTarifaria { get; set; }

    public virtual DbSet<TipoAutorizacion> TipoAutorizacions { get; set; }

    public virtual DbSet<TipoNicho> TipoNichos { get; set; }

    public virtual DbSet<TipoNotum> TipoNota { get; set; }

    public virtual DbSet<TipoNumeracionParcela> TipoNumeracionParcelas { get; set; }

    public virtual DbSet<TipoPanteon> TipoPanteons { get; set; }

    public virtual DbSet<TipoParcela> TipoParcelas { get; set; }

    public virtual DbSet<TipoTramite> TipoTramites { get; set; }

    public virtual DbSet<Tramite> Tramites { get; set; }

    public virtual DbSet<TramitePersona> TramitePersonas { get; set; }

    public virtual DbSet<TramitesCosto> TramitesCostos { get; set; }

    public virtual DbSet<TramitesParcela> TramitesParcelas { get; set; }

    public virtual DbSet<Traslado> Traslados { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AceptacionTitularidad>(entity =>
        {
            entity.HasKey(e => e.TramiteId).HasName("PK__Aceptaci__324535475C5C5AB7");

            entity.ToTable("AceptacionTitularidad");

            entity.Property(e => e.TramiteId)
                .ValueGeneratedNever()
                .HasColumnName("tramiteId");
            entity.Property(e => e.ConcesionId).HasColumnName("concesionId");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.FechaFinalizacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaFinalizacion");
            entity.Property(e => e.InfoAdicional).HasColumnName("infoAdicional");
            entity.Property(e => e.ParcelaId).HasColumnName("parcelaId");
            entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");

            entity.HasOne(d => d.Concesion).WithMany(p => p.AceptacionTitularidadConcesions)
                .HasForeignKey(d => d.ConcesionId)
                .HasConstraintName("FK_AT_concesionId");

            entity.HasOne(d => d.Parcela).WithMany(p => p.AceptacionTitularidads)
                .HasForeignKey(d => d.ParcelaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AT_Parcela");

            entity.HasOne(d => d.Tramite).WithOne(p => p.AceptacionTitularidadTramite)
                .HasForeignKey<AceptacionTitularidad>(d => d.TramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AT_Tramite");

            entity.HasOne(d => d.Usuario).WithMany(p => p.AceptacionTitularidads)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AT_Usuario");
        });

        modelBuilder.Entity<AnioConcesion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AnioConc__3213E83F9C8D39CE");

            entity.ToTable("AnioConcesion");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Anios).HasColumnName("anios");
        });

        modelBuilder.Entity<Archivo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Archivos__3213E83F459F6D1F");

            entity.HasIndex(e => e.Id, "UQ__Archivos__3213E83ECB75E598").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CategoriaArchivo)
                .HasMaxLength(50)
                .HasColumnName("categoriaArchivo");
            entity.Property(e => e.Contenido).HasColumnName("contenido");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.NombreArchivo)
                .HasMaxLength(255)
                .HasColumnName("nombreArchivo");
            entity.Property(e => e.TamanoBytes).HasColumnName("tamanoBytes");
            entity.Property(e => e.TipoArchivo)
                .HasMaxLength(50)
                .HasColumnName("tipoArchivo");
            entity.Property(e => e.TramiteId).HasColumnName("tramiteId");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");

            entity.HasOne(d => d.Tramite).WithMany(p => p.Archivos)
                .HasForeignKey(d => d.TramiteId)
                .HasConstraintName("Archivos_tramiteId_fk");
        });

        modelBuilder.Entity<CambiosTitularidad>(entity =>
        {
            entity.HasKey(e => e.TramiteId).HasName("PK__CambiosT__324535474FCBB6C0");

            entity.ToTable("CambiosTitularidad");

            entity.Property(e => e.TramiteId)
                .ValueGeneratedNever()
                .HasColumnName("tramiteId");
            entity.Property(e => e.ConcesionId).HasColumnName("concesionId");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.FechaFinalizacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaFinalizacion");
            entity.Property(e => e.InfoAdicional).HasColumnName("infoAdicional");
            entity.Property(e => e.ParcelaId).HasColumnName("parcelaId");
            entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");

            entity.HasOne(d => d.Concesion).WithMany(p => p.CambiosTitularidadConcesions)
                .HasForeignKey(d => d.ConcesionId)
                .HasConstraintName("FK_CT_concesionId");

            entity.HasOne(d => d.Parcela).WithMany(p => p.CambiosTitularidads)
                .HasForeignKey(d => d.ParcelaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CT_Parcela");

            entity.HasOne(d => d.Tramite).WithOne(p => p.CambiosTitularidadTramite)
                .HasForeignKey<CambiosTitularidad>(d => d.TramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CT_Tramite");

            entity.HasOne(d => d.Usuario).WithMany(p => p.CambiosTitularidads)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CT_Usuario");
        });

        modelBuilder.Entity<CantidadCuota>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cantidad__3213E83F9BF64100");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cuota).HasColumnName("cuota");
        });

        modelBuilder.Entity<CategoriasPersona>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3213E83F77CD3CBB");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Categoria)
                .HasMaxLength(30)
                .HasColumnName("categoria");
        });

        modelBuilder.Entity<Cementerio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cementer__3213E83FF3E63141");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");
        });

        modelBuilder.Entity<ConceptosTarifarium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Concepto__3213E83F4B30274D");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(70)
                .HasColumnName("nombre");
            entity.Property(e => e.TemaId).HasColumnName("temaId");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");

            entity.HasOne(d => d.Tema).WithMany(p => p.ConceptosTarifaria)
                .HasForeignKey(d => d.TemaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ConceptosTarifaria_temaId_fk");
        });

        modelBuilder.Entity<Concesione>(entity =>
        {
            entity.HasKey(e => e.TramiteId).HasName("PK__Concesio__3245354794DA42D5");

            entity.Property(e => e.TramiteId)
                .ValueGeneratedNever()
                .HasColumnName("tramiteId");
            entity.Property(e => e.CantidadAniosId).HasColumnName("cantidadAniosId");
            entity.Property(e => e.Concesion).HasColumnName("concesion");
            entity.Property(e => e.CuotaId).HasColumnName("cuotaId");
            entity.Property(e => e.InformacionAdicional).HasColumnName("informacionAdicional");
            entity.Property(e => e.ParcelaId).HasColumnName("parcelaId");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("precio");
            entity.Property(e => e.TipoParcela)
                .HasMaxLength(20)
                .HasColumnName("tipoParcela");
            entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");
            entity.Property(e => e.Vencimiento).HasColumnName("vencimiento");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");

            entity.HasOne(d => d.CantidadAnios).WithMany(p => p.Concesiones)
                .HasForeignKey(d => d.CantidadAniosId)
                .HasConstraintName("Concesiones_cantidadAniosId_fk");

            entity.HasOne(d => d.Cuota).WithMany(p => p.Concesiones)
                .HasForeignKey(d => d.CuotaId)
                .HasConstraintName("Concesiones_cuotaId_fk");

            entity.HasOne(d => d.Parcela).WithMany(p => p.Concesiones)
                .HasForeignKey(d => d.ParcelaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Concesiones_parcelaId_fk");

            entity.HasOne(d => d.Tramite).WithOne(p => p.ConcesioneTramite)
                .HasForeignKey<Concesione>(d => d.TramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Concesiones_tramiteId_fk");

            entity.HasOne(d => d.TramiteRetiro).WithMany(p => p.ConcesioneTramiteRetiros)
                .HasForeignKey(d => d.TramiteRetiroId)
                .HasConstraintName("FK_Concesiones_TramiteRetiro");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Concesiones)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("Concesiones_usuarioId_fk");
        });

        modelBuilder.Entity<Cremacione>(entity =>
        {
            entity.HasKey(e => e.TramiteId).HasName("PK__Cremacio__324535475BC2041A");

            entity.Property(e => e.TramiteId)
                .ValueGeneratedNever()
                .HasColumnName("tramiteId");
            entity.Property(e => e.CementerioId).HasColumnName("cementerioId");
            entity.Property(e => e.ConcesionId).HasColumnName("concesionId");
            entity.Property(e => e.Destino)
                .HasMaxLength(150)
                .HasColumnName("destino");
            entity.Property(e => e.DifuntoId).HasColumnName("difuntoId");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.FechaFinalizacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaFinalizacion");
            entity.Property(e => e.FechaPendiente)
                .HasColumnType("datetime")
                .HasColumnName("fechaPendiente");
            entity.Property(e => e.InfoAdicional).HasColumnName("infoAdicional");
            entity.Property(e => e.ParcelaDestinoId).HasColumnName("parcelaDestinoId");
            entity.Property(e => e.ParcelaOrigenId).HasColumnName("parcelaOrigenId");
            entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");

            entity.HasOne(d => d.Cementerio).WithMany(p => p.Cremaciones)
                .HasForeignKey(d => d.CementerioId)
                .HasConstraintName("FK_CREMA_Cementerio");

            entity.HasOne(d => d.Concesion).WithMany(p => p.CremacioneConcesions)
                .HasForeignKey(d => d.ConcesionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CREMA_concesionId");

            entity.HasOne(d => d.Difunto).WithMany(p => p.Cremaciones)
                .HasForeignKey(d => d.DifuntoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CREMA_Difunto");

            entity.HasOne(d => d.ParcelaDestino).WithMany(p => p.CremacioneParcelaDestinos)
                .HasForeignKey(d => d.ParcelaDestinoId)
                .HasConstraintName("FK_CREMA_ParcelaDestino");

            entity.HasOne(d => d.ParcelaOrigen).WithMany(p => p.CremacioneParcelaOrigens)
                .HasForeignKey(d => d.ParcelaOrigenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CREMA_ParcelaOrigen");

            entity.HasOne(d => d.Tramite).WithOne(p => p.CremacioneTramite)
                .HasForeignKey<Cremacione>(d => d.TramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CREMA_Tramite");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Cremaciones)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CREMA_Usuario");
        });

        modelBuilder.Entity<Diagrama>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Diagrama__3214EC07B3269873");

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
        });

        modelBuilder.Entity<DocumentosTramite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Document__3213E83FFC58A458");

            entity.ToTable("DocumentosTramite");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ContenidoHtml).HasColumnName("contenidoHtml");
            entity.Property(e => e.FechaUltimaEdicion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaUltimaEdicion");
            entity.Property(e => e.FirmanteId).HasColumnName("firmanteId");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");
            entity.Property(e => e.Parentesco)
                .HasMaxLength(50)
                .HasColumnName("parentesco");
            entity.Property(e => e.PersonaId).HasColumnName("personaId");
            entity.Property(e => e.PlantillaId).HasColumnName("plantillaId");
            entity.Property(e => e.TipoAutorizacionId).HasColumnName("tipoAutorizacionId");
            entity.Property(e => e.TramiteId).HasColumnName("tramiteId");
            entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");
            entity.Property(e => e.Version)
                .HasDefaultValue(1)
                .HasColumnName("version");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");

            entity.HasOne(d => d.Firmante).WithMany(p => p.DocumentosTramites)
                .HasForeignKey(d => d.FirmanteId)
                .HasConstraintName("FK_DT_Firmante");

            entity.HasOne(d => d.Persona).WithMany(p => p.DocumentosTramites)
                .HasForeignKey(d => d.PersonaId)
                .HasConstraintName("FK_DT_Persona");

            entity.HasOne(d => d.Plantilla).WithMany(p => p.DocumentosTramites)
                .HasForeignKey(d => d.PlantillaId)
                .HasConstraintName("FK_DT_Plantilla");

            entity.HasOne(d => d.TipoAutorizacion).WithMany(p => p.DocumentosTramites)
                .HasForeignKey(d => d.TipoAutorizacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DT_tipoAutorizacion");

            entity.HasOne(d => d.Tramite).WithMany(p => p.DocumentosTramites)
                .HasForeignKey(d => d.TramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DT_Tramite");

            entity.HasOne(d => d.Usuario).WithMany(p => p.DocumentosTramites)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DT_Usuario");
        });

        modelBuilder.Entity<EmpresasFunebre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Empresas__3213E83F9B3F5606");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");
        });

        modelBuilder.Entity<EstadosDifunto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EstadosD__3213E83F63E2072B");

            entity.ToTable("EstadosDifunto");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .HasColumnName("estado");
        });

        modelBuilder.Entity<EstadosTramite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EstadosT__3213E83F45A865D7");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .HasColumnName("estado");
            entity.Property(e => e.TipoTramiteId).HasColumnName("tipoTramiteId");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");

            entity.HasOne(d => d.TipoTramite).WithMany(p => p.EstadosTramites)
                .HasForeignKey(d => d.TipoTramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("EstadosTramites_tipoTramiteId_fk");
        });

        modelBuilder.Entity<EventoCalendario>(entity =>
        {
            entity.ToTable("EventoCalendario");

            entity.Property(e => e.Titulo).HasMaxLength(200);
        });

        modelBuilder.Entity<FirmantesTramite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Firmante__3213E83FC197C42E");

            entity.ToTable("FirmantesTramite");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EsTitular).HasColumnName("esTitular");
            entity.Property(e => e.FechaAlta)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaAlta");
            entity.Property(e => e.Parentesco)
                .HasMaxLength(50)
                .HasColumnName("parentesco");
            entity.Property(e => e.PersonaId).HasColumnName("personaId");
            entity.Property(e => e.TramiteId).HasColumnName("tramiteId");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");

            entity.HasOne(d => d.Persona).WithMany(p => p.FirmantesTramites)
                .HasForeignKey(d => d.PersonaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FT_Persona");

            entity.HasOne(d => d.Tramite).WithMany(p => p.FirmantesTramites)
                .HasForeignKey(d => d.TramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FT_Tramite");
        });

        modelBuilder.Entity<HistorialEstadoTramite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Historia__3213E83F5F6707E9");

            entity.ToTable("HistorialEstadoTramite");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EstadoTramiteId).HasColumnName("estadoTramiteId");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.TramiteId).HasColumnName("tramiteId");

            entity.HasOne(d => d.EstadoTramite).WithMany(p => p.HistorialEstadoTramites)
                .HasForeignKey(d => d.EstadoTramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("HistorialEstadoTramite_estadoTramiteId_fk");

            entity.HasOne(d => d.Tramite).WithMany(p => p.HistorialEstadoTramites)
                .HasForeignKey(d => d.TramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("HistorialEstadoTramite_tramiteId_fk");
        });

        modelBuilder.Entity<HistorialParcelasConcesion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Historia__3213E83FC7DE3D70");

            entity.ToTable("HistorialParcelasConcesion");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConcesionId).HasColumnName("concesionId");
            entity.Property(e => e.FechaFin).HasColumnName("fechaFin");
            entity.Property(e => e.FechaInicio).HasColumnName("fechaInicio");
            entity.Property(e => e.ParcelaId).HasColumnName("parcelaId");
            entity.Property(e => e.TramiteOrigenId).HasColumnName("tramiteOrigenId");

            entity.HasOne(d => d.Concesion).WithMany(p => p.HistorialParcelasConcesions)
                .HasForeignKey(d => d.ConcesionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HPC_Concesion");

            entity.HasOne(d => d.Parcela).WithMany(p => p.HistorialParcelasConcesions)
                .HasForeignKey(d => d.ParcelaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HPC_Parcela");

            entity.HasOne(d => d.TramiteOrigen).WithMany(p => p.HistorialParcelasConcesions)
                .HasForeignKey(d => d.TramiteOrigenId)
                .HasConstraintName("FK_HPC_Tramite");
        });

        modelBuilder.Entity<HistorialTitularesConcesione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Historia__3213E83FF8C63320");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConcesionId).HasColumnName("concesionId");
            entity.Property(e => e.FechaFin)
                .HasColumnType("datetime")
                .HasColumnName("fechaFin");
            entity.Property(e => e.FechaInicio)
                .HasColumnType("datetime")
                .HasColumnName("fechaInicio");
            entity.Property(e => e.PersonaId).HasColumnName("personaId");

            entity.HasOne(d => d.Concesion).WithMany(p => p.HistorialTitularesConcesiones)
                .HasForeignKey(d => d.ConcesionId)
                .HasConstraintName("HistorialTitularesConcesiones_concesionId_fk");

            entity.HasOne(d => d.Persona).WithMany(p => p.HistorialTitularesConcesiones)
                .HasForeignKey(d => d.PersonaId)
                .HasConstraintName("HistorialTitularesConcesiones_personaId_fk");
        });

        modelBuilder.Entity<Introduccione>(entity =>
        {
            entity.HasKey(e => e.TramiteId).HasName("PK__Introduc__3245354774BB66DF");

            entity.Property(e => e.TramiteId)
                .ValueGeneratedNever()
                .HasColumnName("tramiteId");
            entity.Property(e => e.DifuntoId).HasColumnName("difuntoId");
            entity.Property(e => e.EmpresaFunebreId).HasColumnName("empresaFunebreId");
            entity.Property(e => e.EstadoDifuntoId).HasColumnName("estadoDifuntoId");
            entity.Property(e => e.FechaIngreso)
                .HasColumnType("datetime")
                .HasColumnName("fechaIngreso");
            entity.Property(e => e.InformacionAdicional).HasColumnName("informacionAdicional");
            entity.Property(e => e.ParcelaId).HasColumnName("parcelaId");
            entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");

            entity.HasOne(d => d.Difunto).WithMany(p => p.Introducciones)
                .HasForeignKey(d => d.DifuntoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Introducciones_difuntoId_fk");

            entity.HasOne(d => d.EmpresaFunebre).WithMany(p => p.Introducciones)
                .HasForeignKey(d => d.EmpresaFunebreId)
                .HasConstraintName("Introducciones_empresaFunebreId_fk");

            entity.HasOne(d => d.EstadoDifunto).WithMany(p => p.Introducciones)
                .HasForeignKey(d => d.EstadoDifuntoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Introducc__estad__1EA48E88");

            entity.HasOne(d => d.Parcela).WithMany(p => p.Introducciones)
                .HasForeignKey(d => d.ParcelaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Introducciones_parcelaId_fk");

            entity.HasOne(d => d.Tramite).WithOne(p => p.Introduccione)
                .HasForeignKey<Introduccione>(d => d.TramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Introducciones_tramiteId_fk");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Introducciones)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Introducciones_usuarioId_fk");
        });

        modelBuilder.Entity<Nota>(entity =>
        {
            entity.HasKey(e => e.TramiteId).HasName("PK__Notas__32453547FF7292AC");

            entity.Property(e => e.TramiteId)
                .ValueGeneratedNever()
                .HasColumnName("tramiteId");
            entity.Property(e => e.Color)
                .HasMaxLength(16)
                .HasColumnName("color");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.TipoNotaId).HasColumnName("tipoNotaId");
            entity.Property(e => e.TramiteIngresoId).HasColumnName("tramiteIngresoId");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");

            entity.HasOne(d => d.TipoNota).WithMany(p => p.Nota)
                .HasForeignKey(d => d.TipoNotaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Notas_tipoNotaId_fk");

            entity.HasOne(d => d.Tramite).WithOne(p => p.NotaTramite)
                .HasForeignKey<Nota>(d => d.TramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notas__tramiteId__71D1E811");

            entity.HasOne(d => d.TramiteIngreso).WithMany(p => p.NotaTramiteIngresos)
                .HasForeignKey(d => d.TramiteIngresoId)
                .HasConstraintName("FK__Notas__tramiteIn__72C60C4A");
        });

        modelBuilder.Entity<Parcela>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Parcelas__3213E83FA3E2EC20");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CantidadDifuntos).HasColumnName("cantidadDifuntos");
            entity.Property(e => e.InformacionAdicional).HasColumnName("informacionAdicional");
            entity.Property(e => e.NombrePanteon)
                .HasMaxLength(50)
                .HasColumnName("nombrePanteon");
            entity.Property(e => e.NroFila).HasColumnName("nroFila");
            entity.Property(e => e.NroParcela).HasColumnName("nroParcela");
            entity.Property(e => e.SeccionId).HasColumnName("seccionId");
            entity.Property(e => e.TipoNichoId).HasColumnName("tipoNichoId");
            entity.Property(e => e.TipoPanteonId).HasColumnName("tipoPanteonId");
            entity.Property(e => e.TipoParcelaId).HasColumnName("tipoParcelaId");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");

            entity.HasOne(d => d.Seccion).WithMany(p => p.Parcelas)
                .HasForeignKey(d => d.SeccionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Parcelas_seccionId_fk");

            entity.HasOne(d => d.TipoNicho).WithMany(p => p.Parcelas)
                .HasForeignKey(d => d.TipoNichoId)
                .HasConstraintName("Parcelas_tipoNichoId_fk");

            entity.HasOne(d => d.TipoPanteon).WithMany(p => p.Parcelas)
                .HasForeignKey(d => d.TipoPanteonId)
                .HasConstraintName("Parcelas_tipoPanteonId_fk");

            entity.HasOne(d => d.TipoParcela).WithMany(p => p.Parcelas)
                .HasForeignKey(d => d.TipoParcelaId)
                .HasConstraintName("Parcelas_tipoParcelaId_fk");
        });

        modelBuilder.Entity<ParcelaDifunto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ParcelaD__3213E83F25A10CAA");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DifuntoId).HasColumnName("difuntoId");
            entity.Property(e => e.FechaIngreso)
                .HasColumnType("datetime")
                .HasColumnName("fechaIngreso");
            entity.Property(e => e.FechaRetiro)
                .HasColumnType("datetime")
                .HasColumnName("fechaRetiro");
            entity.Property(e => e.ParcelaId).HasColumnName("parcelaId");
            entity.Property(e => e.TramiteIngresoId).HasColumnName("tramiteIngresoId");
            entity.Property(e => e.TramiteRetiroId).HasColumnName("tramiteRetiroId");

            entity.HasOne(d => d.Difunto).WithMany(p => p.ParcelaDifuntos)
                .HasForeignKey(d => d.DifuntoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ParcelaDifuntos_difuntoId_fk");

            entity.HasOne(d => d.Parcela).WithMany(p => p.ParcelaDifuntos)
                .HasForeignKey(d => d.ParcelaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ParcelaDifuntos_parcelaId_fk");

            entity.HasOne(d => d.TramiteIngreso).WithMany(p => p.ParcelaDifuntoTramiteIngresos)
                .HasForeignKey(d => d.TramiteIngresoId)
                .HasConstraintName("ParcelaDifuntos_tramiteIngresoId_fk");

            entity.HasOne(d => d.TramiteRetiro).WithMany(p => p.ParcelaDifuntoTramiteRetiros)
                .HasForeignKey(d => d.TramiteRetiroId)
                .HasConstraintName("ParcelaDifuntos_tramiteRetiroId_fk");
        });

        modelBuilder.Entity<PermisosIngreso>(entity =>
        {
            entity.HasKey(e => e.TramiteId).HasName("PK__Permisos__3245354791A58F7F");

            entity.Property(e => e.TramiteId)
                .ValueGeneratedNever()
                .HasColumnName("tramiteId");
            entity.Property(e => e.ConcesionId).HasColumnName("concesionId");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.FechaFinalizacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaFinalizacion");
            entity.Property(e => e.NombreFallecido).HasColumnName("nombreFallecido");
            entity.Property(e => e.ParcelaId).HasColumnName("parcelaId");
            entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");

            entity.HasOne(d => d.Concesion).WithMany(p => p.PermisosIngresoConcesions)
                .HasForeignKey(d => d.ConcesionId)
                .HasConstraintName("FK_PERMISO_concesionId");

            entity.HasOne(d => d.Parcela).WithMany(p => p.PermisosIngresos)
                .HasForeignKey(d => d.ParcelaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PERMISO_Parcela");

            entity.HasOne(d => d.Tramite).WithOne(p => p.PermisosIngresoTramite)
                .HasForeignKey<PermisosIngreso>(d => d.TramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PERMISO_Tramite");

            entity.HasOne(d => d.Usuario).WithMany(p => p.PermisosIngresos)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PERMISO_Usuario");
        });

        modelBuilder.Entity<PermisosRefaccione>(entity =>
        {
            entity.HasKey(e => e.TramiteId).HasName("PK__Permisos__324535475DE06CED");

            entity.Property(e => e.TramiteId)
                .ValueGeneratedNever()
                .HasColumnName("tramiteId");
            entity.Property(e => e.ConcesionId).HasColumnName("concesionId");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.FechaFinalizacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaFinalizacion");
            entity.Property(e => e.FechaPendiente)
                .HasColumnType("datetime")
                .HasColumnName("fechaPendiente");
            entity.Property(e => e.ParcelaId).HasColumnName("parcelaId");
            entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");

            entity.HasOne(d => d.Concesion).WithMany(p => p.PermisosRefaccioneConcesions)
                .HasForeignKey(d => d.ConcesionId)
                .HasConstraintName("FK_PERMISOREFACCION_concesionId");

            entity.HasOne(d => d.Parcela).WithMany(p => p.PermisosRefacciones)
                .HasForeignKey(d => d.ParcelaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PERMISOREFACCION_Parcela");

            entity.HasOne(d => d.Tramite).WithOne(p => p.PermisosRefaccioneTramite)
                .HasForeignKey<PermisosRefaccione>(d => d.TramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PERMISOREFACCION_Tramite");

            entity.HasOne(d => d.Usuario).WithMany(p => p.PermisosRefacciones)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PERMISOREFACCION_Usuario");
        });

        modelBuilder.Entity<Persona>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Personas__3213E83F4A7D3E7A");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .HasColumnName("apellido");
            entity.Property(e => e.CategoriaPersonaId).HasColumnName("categoriaPersonaId");
            entity.Property(e => e.Celular)
                .HasMaxLength(50)
                .HasColumnName("celular");
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .HasColumnName("correo");
            entity.Property(e => e.Dni)
                .HasMaxLength(15)
                .HasColumnName("dni");
            entity.Property(e => e.Domicilio)
                .HasMaxLength(100)
                .HasColumnName("domicilio");
            entity.Property(e => e.EstadoDifuntoId).HasColumnName("estadoDifuntoId");
            entity.Property(e => e.FechaDefuncion).HasColumnName("fechaDefuncion");
            entity.Property(e => e.FechaNacimiento).HasColumnName("fechaNacimiento");
            entity.Property(e => e.InformacionAdicional).HasColumnName("informacionAdicional");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.NroActa).HasColumnName("nroActa");
            entity.Property(e => e.NroAge).HasColumnName("nroAge");
            entity.Property(e => e.NroFolio).HasColumnName("nroFolio");
            entity.Property(e => e.NroSerie)
                .HasMaxLength(5)
                .HasColumnName("nroSerie");
            entity.Property(e => e.NroTomo).HasColumnName("nroTomo");
            entity.Property(e => e.Sexo)
                .HasMaxLength(15)
                .HasColumnName("sexo");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");

            entity.HasOne(d => d.CategoriaPersona).WithMany(p => p.Personas)
                .HasForeignKey(d => d.CategoriaPersonaId)
                .HasConstraintName("Personas_categoriaPersonaId_fk");

            entity.HasOne(d => d.EstadoDifunto).WithMany(p => p.Personas)
                .HasForeignKey(d => d.EstadoDifuntoId)
                .HasConstraintName("Personas_estadoDifuntoId_fk");
        });

        modelBuilder.Entity<PlantillasTramite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Plantill__3213E83F9221BF82");

            entity.ToTable("PlantillasTramite");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Contenido).HasColumnName("contenido");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaModificacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.TipoAutorizacionId).HasColumnName("tipoAutorizacionId");
            entity.Property(e => e.TipoTramiteId).HasColumnName("tipoTramiteId");

            entity.HasOne(d => d.TipoAutorizacion).WithMany(p => p.PlantillasTramites)
                .HasForeignKey(d => d.TipoAutorizacionId)
                .HasConstraintName("FK_PA_TipoAutorizacion");

            entity.HasOne(d => d.TipoTramite).WithMany(p => p.PlantillasTramites)
                .HasForeignKey(d => d.TipoTramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PT_TipoTramite");
        });

        modelBuilder.Entity<PreciosTarifaria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PreciosT__3213E83F7C4C2FFC");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AniosConcesionId).HasColumnName("aniosConcesionId");
            entity.Property(e => e.ConceptoTarifariaId).HasColumnName("conceptoTarifariaId");
            entity.Property(e => e.NroFila).HasColumnName("nroFila");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("precio");
            entity.Property(e => e.SeccionId).HasColumnName("seccionId");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");

            entity.HasOne(d => d.ConceptoTarifaria).WithMany(p => p.PreciosTarifaria)
                .HasForeignKey(d => d.ConceptoTarifariaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PreciosTarifarias_conceptoTarifariaId_fk");

            entity.HasOne(d => d.Seccion).WithMany(p => p.PreciosTarifaria)
                .HasForeignKey(d => d.SeccionId)
                .HasConstraintName("PreciosTarifarias_seccionId_fk");
        });

        modelBuilder.Entity<Reduccione>(entity =>
        {
            entity.HasKey(e => e.TramiteId).HasName("PK__Reduccio__32453547753DDBEF");

            entity.Property(e => e.TramiteId)
                .ValueGeneratedNever()
                .HasColumnName("tramiteId");
            entity.Property(e => e.CementerioId).HasColumnName("cementerioId");
            entity.Property(e => e.ConcesionId).HasColumnName("concesionId");
            entity.Property(e => e.Destino)
                .HasMaxLength(150)
                .HasColumnName("destino");
            entity.Property(e => e.DifuntoId).HasColumnName("difuntoId");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.FechaFinalizacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaFinalizacion");
            entity.Property(e => e.FechaPendiente)
                .HasColumnType("datetime")
                .HasColumnName("fechaPendiente");
            entity.Property(e => e.InfoAdicional).HasColumnName("infoAdicional");
            entity.Property(e => e.ParcelaDestinoId).HasColumnName("parcelaDestinoId");
            entity.Property(e => e.ParcelaOrigenId).HasColumnName("parcelaOrigenId");
            entity.Property(e => e.TipoTraslado).HasColumnName("tipoTraslado");
            entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");

            entity.HasOne(d => d.Cementerio).WithMany(p => p.Reducciones)
                .HasForeignKey(d => d.CementerioId)
                .HasConstraintName("FK_REDUCCION_Cementerio");

            entity.HasOne(d => d.Concesion).WithMany(p => p.ReduccioneConcesions)
                .HasForeignKey(d => d.ConcesionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_REDUCCION_concesionId");

            entity.HasOne(d => d.Difunto).WithMany(p => p.Reducciones)
                .HasForeignKey(d => d.DifuntoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_REDUCCION_Difunto");

            entity.HasOne(d => d.ParcelaDestino).WithMany(p => p.ReduccioneParcelaDestinos)
                .HasForeignKey(d => d.ParcelaDestinoId)
                .HasConstraintName("FK_REDUCCION_ParcelaDestino");

            entity.HasOne(d => d.ParcelaOrigen).WithMany(p => p.ReduccioneParcelaOrigens)
                .HasForeignKey(d => d.ParcelaOrigenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_REDUCCION_ParcelaOrigen");

            entity.HasOne(d => d.Tramite).WithOne(p => p.ReduccioneTramite)
                .HasForeignKey<Reduccione>(d => d.TramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_REDUCCION_Tramite");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Reducciones)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_REDUCCION_Usuario");
        });

        modelBuilder.Entity<ReglasIngreso>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ReglasIn__3213E83F713F85D0");

            entity.ToTable("ReglasIngreso");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CierreFosa).HasColumnName("cierreFosa");
            entity.Property(e => e.CierreNicho).HasColumnName("cierreNicho");
            entity.Property(e => e.ConceptoDefuncionId).HasColumnName("conceptoDefuncionId");
            entity.Property(e => e.ConceptoInhumacionId).HasColumnName("conceptoInhumacionId");
            entity.Property(e => e.ConceptoIntroduccionId).HasColumnName("conceptoIntroduccionId");
            entity.Property(e => e.ConceptoTranscripcionId).HasColumnName("conceptoTranscripcionId");
            entity.Property(e => e.EstadoDifuntoId).HasColumnName("estadoDifuntoId");
            entity.Property(e => e.MontoMinimoFondoId).HasColumnName("montoMinimoFondoId");
            entity.Property(e => e.NombreRegla)
                .HasMaxLength(100)
                .HasColumnName("nombreRegla");
            entity.Property(e => e.PorcentajeAumentoDerechoOficinaId).HasColumnName("porcentajeAumentoDerechoOficinaId");
            entity.Property(e => e.PorcentajeAumentoOtraLocalidadId).HasColumnName("porcentajeAumentoOtraLocalidadId");
            entity.Property(e => e.PorcentajeFondoSaludId).HasColumnName("porcentajeFondoSaludId");
            entity.Property(e => e.PorcentajeIntroduccionUrnaDerechoOficna).HasColumnName("porcentajeIntroduccionUrnaDerechoOficna");
            entity.Property(e => e.TipoNichoId).HasColumnName("tipoNichoId");
            entity.Property(e => e.TipoPanteonId).HasColumnName("tipoPanteonId");
            entity.Property(e => e.TipoParcelaId).HasColumnName("tipoParcelaId");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");

            entity.HasOne(d => d.CierreFosaNavigation).WithMany(p => p.ReglasIngresoCierreFosaNavigations)
                .HasForeignKey(d => d.CierreFosa)
                .HasConstraintName("FK_RI_CierreFosa");

            entity.HasOne(d => d.CierreNichoNavigation).WithMany(p => p.ReglasIngresoCierreNichoNavigations)
                .HasForeignKey(d => d.CierreNicho)
                .HasConstraintName("FK_RI_CierreNicho");

            entity.HasOne(d => d.ConceptoDefuncion).WithMany(p => p.ReglasIngresoConceptoDefuncions)
                .HasForeignKey(d => d.ConceptoDefuncionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RI_ConceptoDefuncion");

            entity.HasOne(d => d.ConceptoInhumacion).WithMany(p => p.ReglasIngresoConceptoInhumacions)
                .HasForeignKey(d => d.ConceptoInhumacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RI_ConceptoInhumacion");

            entity.HasOne(d => d.ConceptoIntroduccion).WithMany(p => p.ReglasIngresoConceptoIntroduccions)
                .HasForeignKey(d => d.ConceptoIntroduccionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RI_ConceptoIntroduccion");

            entity.HasOne(d => d.ConceptoTranscripcion).WithMany(p => p.ReglasIngresoConceptoTranscripcions)
                .HasForeignKey(d => d.ConceptoTranscripcionId)
                .HasConstraintName("FK_RI_ConceptoTranscripcion");

            entity.HasOne(d => d.EstadoDifunto).WithMany(p => p.ReglasIngresos)
                .HasForeignKey(d => d.EstadoDifuntoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RI_EstadoDifunto");

            entity.HasOne(d => d.MontoMinimoFondo).WithMany(p => p.ReglasIngresoMontoMinimoFondos)
                .HasForeignKey(d => d.MontoMinimoFondoId)
                .HasConstraintName("FK_RI_MontoMinimoFondo");

            entity.HasOne(d => d.PorcentajeAumentoDerechoOficina).WithMany(p => p.ReglasIngresoPorcentajeAumentoDerechoOficinas)
                .HasForeignKey(d => d.PorcentajeAumentoDerechoOficinaId)
                .HasConstraintName("FK_RI_PorcAumentoDerechoOficina");

            entity.HasOne(d => d.PorcentajeAumentoOtraLocalidad).WithMany(p => p.ReglasIngresoPorcentajeAumentoOtraLocalidads)
                .HasForeignKey(d => d.PorcentajeAumentoOtraLocalidadId)
                .HasConstraintName("FK_RI_PorcAumentoOtraLocalidad");

            entity.HasOne(d => d.PorcentajeFondoSalud).WithMany(p => p.ReglasIngresoPorcentajeFondoSaluds)
                .HasForeignKey(d => d.PorcentajeFondoSaludId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RI_PorcFondoSalud");

            entity.HasOne(d => d.PorcentajeIntroduccionUrnaDerechoOficnaNavigation).WithMany(p => p.ReglasIngresoPorcentajeIntroduccionUrnaDerechoOficnaNavigations)
                .HasForeignKey(d => d.PorcentajeIntroduccionUrnaDerechoOficna)
                .HasConstraintName("FK_RI_PorcIntroduccionUrnaDO");

            entity.HasOne(d => d.TipoNicho).WithMany(p => p.ReglasIngresos)
                .HasForeignKey(d => d.TipoNichoId)
                .HasConstraintName("FK_RI_TipoNicho");

            entity.HasOne(d => d.TipoPanteon).WithMany(p => p.ReglasIngresos)
                .HasForeignKey(d => d.TipoPanteonId)
                .HasConstraintName("FK_RI_TipoPanteon");

            entity.HasOne(d => d.TipoParcela).WithMany(p => p.ReglasIngresos)
                .HasForeignKey(d => d.TipoParcelaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RI_TipoParcela");
        });

        modelBuilder.Entity<RequisitosTramite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Requisit__3213E83FB30338E1");

            entity.ToTable("RequisitosTramite");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.TipoTramiteId).HasColumnName("tipoTramiteId");

            entity.HasOne(d => d.TipoTramite).WithMany(p => p.RequisitosTramites)
                .HasForeignKey(d => d.TipoTramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RT_TipoTramite");
        });

        modelBuilder.Entity<RolesUsuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RolesUsu__3213E83FFFB18A43");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Rol)
                .HasMaxLength(30)
                .HasColumnName("rol");
        });

        modelBuilder.Entity<Seccione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Seccione__3213E83F46619BA2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Filas).HasColumnName("filas");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.NroParcelas).HasColumnName("nroParcelas");
            entity.Property(e => e.TipoNumeracionParcelaId).HasColumnName("tipoNumeracionParcelaId");
            entity.Property(e => e.TipoParcelaId).HasColumnName("tipoParcelaId");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");

            entity.HasOne(d => d.TipoNumeracionParcela).WithMany(p => p.Secciones)
                .HasForeignKey(d => d.TipoNumeracionParcelaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Secciones_tipoNumeracionParcelaId_fk");

            entity.HasOne(d => d.TipoParcela).WithMany(p => p.Secciones)
                .HasForeignKey(d => d.TipoParcelaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Secciones_tipoParcelaId_fk");
        });

        modelBuilder.Entity<Tarea>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tareas__3213E83F450F1B0C");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.NotaId).HasColumnName("notaId");
            entity.Property(e => e.TramiteId).HasColumnName("tramiteId");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");

            entity.HasOne(d => d.Nota).WithMany(p => p.Tareas)
                .HasForeignKey(d => d.NotaId)
                .HasConstraintName("Tareas_notaId_fk");

            entity.HasOne(d => d.TareaPlantilla).WithMany(p => p.Tareas)
                .HasForeignKey(d => d.TareaPlantillaId)
                .HasConstraintName("FK_Tareas_TareaPlantilla");

            entity.HasOne(d => d.Tramite).WithMany(p => p.Tareas)
                .HasForeignKey(d => d.TramiteId)
                .HasConstraintName("Tareas_tramiteId_fk");
        });

        modelBuilder.Entity<TareaPlantilla>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TareaPla__3214EC074D43805B");

            entity.ToTable("TareaPlantilla");

            entity.Property(e => e.Descripcion).HasMaxLength(150);
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.Visibilidad).HasDefaultValue(true);

            entity.HasOne(d => d.TipoTramite).WithMany(p => p.TareaPlantillas)
                .HasForeignKey(d => d.TipoTramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TareaPlantilla_TipoTramiteId_fk");
        });

        modelBuilder.Entity<TemasTarifarium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TemasTar__3213E83F085DF1C2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");
        });

        modelBuilder.Entity<TipoAutorizacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TipoAuto__3213E83FB68A41AD");

            entity.ToTable("TipoAutorizacion");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.TipoTramiteId).HasColumnName("tipoTramiteId");

            entity.HasOne(d => d.TipoTramite).WithMany(p => p.TipoAutorizacions)
                .HasForeignKey(d => d.TipoTramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PA_TipoTramite");
        });

        modelBuilder.Entity<TipoNicho>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TipoNich__3213E83F4C98C0B1");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<TipoNotum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TipoNota__3213E83F9AEC39D2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion).HasMaxLength(30);
            entity.Property(e => e.Visibilidad).HasColumnName("visibilidad");
        });

        modelBuilder.Entity<TipoNumeracionParcela>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TipoNume__3213E83F711B1CAB");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TipoNumeracion)
                .HasMaxLength(30)
                .HasColumnName("tipoNumeracion");
        });

        modelBuilder.Entity<TipoPanteon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TipoPant__3213E83F3183155F");

            entity.ToTable("TipoPanteon");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<TipoParcela>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TipoParc__3213E83FF62FF578");

            entity.ToTable("TipoParcela");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Tipo)
                .HasMaxLength(30)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<TipoTramite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TipoTram__3213E83F684E8EE8");

            entity.ToTable("TipoTramite");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .HasColumnName("tipo");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");
        });

        modelBuilder.Entity<Tramite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tramites__3213E83F4DD691AE");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.EstadoActualId).HasColumnName("estadoActualId");
            entity.Property(e => e.FechaCreacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.FechaFinalizacion).HasColumnType("datetime");
            entity.Property(e => e.TipoTramiteId).HasColumnName("tipoTramiteId");
            entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");

            entity.HasOne(d => d.EstadoActual).WithMany(p => p.Tramites)
                .HasForeignKey(d => d.EstadoActualId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Tramites_estadoActualId_fk");

            entity.HasOne(d => d.TipoTramite).WithMany(p => p.Tramites)
                .HasForeignKey(d => d.TipoTramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Tramites_tipoTramiteId_fk");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Tramites)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Tramites_usuarioId_fk");
        });

        modelBuilder.Entity<TramitePersona>(entity =>
        {
            entity.HasKey(e => new { e.TramiteId, e.PersonaId }).HasName("PK__TramiteP__770E40CA9CEFA5CB");

            entity.ToTable("TramitePersona");

            entity.Property(e => e.TramiteId).HasColumnName("tramiteId");
            entity.Property(e => e.PersonaId).HasColumnName("personaId");
            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");

            entity.HasOne(d => d.Persona).WithMany(p => p.TramitePersonas)
                .HasForeignKey(d => d.PersonaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TramitePersona_personaId_fk");

            entity.HasOne(d => d.Tramite).WithMany(p => p.TramitePersonas)
                .HasForeignKey(d => d.TramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TramitePersona_tramiteId_fk");
        });

        modelBuilder.Entity<TramitesCosto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tramites__3213E83FFEBAAA92");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConceptoTarifariaId).HasColumnName("conceptoTarifariaId");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("monto");
            entity.Property(e => e.TramiteId).HasColumnName("tramiteId");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");

            entity.HasOne(d => d.ConceptoTarifaria).WithMany(p => p.TramitesCostos)
                .HasForeignKey(d => d.ConceptoTarifariaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TC_Concepto");

            entity.HasOne(d => d.Tramite).WithMany(p => p.TramitesCostos)
                .HasForeignKey(d => d.TramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TC_Tramite");
        });

        modelBuilder.Entity<TramitesParcela>(entity =>
        {
            entity.HasKey(e => new { e.TramiteId, e.ParcelaId }).HasName("PK__Tramites__47EF37ADAA207313");

            entity.ToTable("TramitesParcela");

            entity.Property(e => e.TramiteId).HasColumnName("tramiteId");
            entity.Property(e => e.ParcelaId).HasColumnName("parcelaId");
            entity.Property(e => e.FechaRegistro).HasColumnName("fechaRegistro");

            entity.HasOne(d => d.Parcela).WithMany(p => p.TramitesParcelas)
                .HasForeignKey(d => d.ParcelaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TramitesParcela_parcelaId_fk");

            entity.HasOne(d => d.Tramite).WithMany(p => p.TramitesParcelas)
                .HasForeignKey(d => d.TramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TramitesParcela_tramiteId_fk");
        });

        modelBuilder.Entity<Traslado>(entity =>
        {
            entity.HasKey(e => e.TramiteId).HasName("PK__Traslado__3245354735AD0605");

            entity.Property(e => e.TramiteId)
                .ValueGeneratedNever()
                .HasColumnName("tramiteId");
            entity.Property(e => e.CementerioId).HasColumnName("cementerioId");
            entity.Property(e => e.ConcesionId).HasColumnName("concesionId");
            entity.Property(e => e.Destino)
                .HasMaxLength(150)
                .HasColumnName("destino");
            entity.Property(e => e.DifuntoId).HasColumnName("difuntoId");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.FechaFinalizacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaFinalizacion");
            entity.Property(e => e.FechaPendiente)
                .HasColumnType("datetime")
                .HasColumnName("fechaPendiente");
            entity.Property(e => e.InfoAdicional).HasColumnName("infoAdicional");
            entity.Property(e => e.ParcelaDestinoId).HasColumnName("parcelaDestinoId");
            entity.Property(e => e.ParcelaOrigenId).HasColumnName("parcelaOrigenId");
            entity.Property(e => e.TipoTraslado).HasColumnName("tipoTraslado");
            entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");

            entity.HasOne(d => d.Cementerio).WithMany(p => p.Traslados)
                .HasForeignKey(d => d.CementerioId)
                .HasConstraintName("FK_TRASLADO_Cementerio");

            entity.HasOne(d => d.Concesion).WithMany(p => p.TrasladoConcesions)
                .HasForeignKey(d => d.ConcesionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TRASLADO_concesionId");

            entity.HasOne(d => d.Difunto).WithMany(p => p.Traslados)
                .HasForeignKey(d => d.DifuntoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TRASLADO_Difunto");

            entity.HasOne(d => d.ParcelaDestino).WithMany(p => p.TrasladoParcelaDestinos)
                .HasForeignKey(d => d.ParcelaDestinoId)
                .HasConstraintName("FK_TRASLADO_ParcelaDestino");

            entity.HasOne(d => d.ParcelaOrigen).WithMany(p => p.TrasladoParcelaOrigens)
                .HasForeignKey(d => d.ParcelaOrigenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TRASLADO_ParcelaOrigen");

            entity.HasOne(d => d.Tramite).WithOne(p => p.TrasladoTramite)
                .HasForeignKey<Traslado>(d => d.TramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TRASLADO_Tramite");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Traslados)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TRASLADO_Usuario");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3213E83F7FCFC59A");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .HasColumnName("apellido");
            entity.Property(e => e.Clave).HasColumnName("clave");
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .HasColumnName("correo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.RolId).HasColumnName("rolId");
            entity.Property(e => e.Usuario1)
                .HasMaxLength(50)
                .HasColumnName("usuario");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");

            entity.HasOne(d => d.Rol).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Usuarios_rol_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
