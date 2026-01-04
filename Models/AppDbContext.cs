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

    public virtual DbSet<AnioConcesion> AnioConcesions { get; set; }

    public virtual DbSet<Archivo> Archivos { get; set; }

    public virtual DbSet<CantidadCuota> CantidadCuotas { get; set; }

    public virtual DbSet<CategoriasPersona> CategoriasPersonas { get; set; }

    public virtual DbSet<Cementerio> Cementerios { get; set; }

    public virtual DbSet<ConceptosTarifarium> ConceptosTarifaria { get; set; }

    public virtual DbSet<Concesione> Concesiones { get; set; }

    public virtual DbSet<EmpresasFunebre> EmpresasFunebres { get; set; }

    public virtual DbSet<EstadosDifunto> EstadosDifuntos { get; set; }

    public virtual DbSet<EstadosTramite> EstadosTramites { get; set; }

    public virtual DbSet<HistorialEstadoTramite> HistorialEstadoTramites { get; set; }

    public virtual DbSet<HistorialTitularesConcesione> HistorialTitularesConcesiones { get; set; }

    public virtual DbSet<Introduccione> Introducciones { get; set; }

    public virtual DbSet<Nota> Notas { get; set; }

    public virtual DbSet<Parcela> Parcelas { get; set; }

    public virtual DbSet<ParcelaDifunto> ParcelaDifuntos { get; set; }

    public virtual DbSet<Persona> Personas { get; set; }

    public virtual DbSet<PreciosTarifaria> PreciosTarifarias { get; set; }

    public virtual DbSet<RolesUsuario> RolesUsuarios { get; set; }

    public virtual DbSet<Seccione> Secciones { get; set; }

    public virtual DbSet<Tarea> Tareas { get; set; }

    public virtual DbSet<TemasTarifarium> TemasTarifaria { get; set; }

    public virtual DbSet<TipoNicho> TipoNichos { get; set; }

    public virtual DbSet<TipoNotum> TipoNota { get; set; }

    public virtual DbSet<TipoNumeracionParcela> TipoNumeracionParcelas { get; set; }

    public virtual DbSet<TipoPanteon> TipoPanteons { get; set; }

    public virtual DbSet<TipoParcela> TipoParcelas { get; set; }

    public virtual DbSet<TipoTramite> TipoTramites { get; set; }

    public virtual DbSet<Tramite> Tramites { get; set; }

    public virtual DbSet<TramitePersona> TramitePersonas { get; set; }

    public virtual DbSet<TramitesParcela> TramitesParcelas { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnioConcesion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AnioConc__3213E83F4A73FB53");

            entity.ToTable("AnioConcesion");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Anios).HasColumnName("anios");
        });

        modelBuilder.Entity<Archivo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Archivos__3213E83FDEDBEEEB");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CategoriaArchivo)
                .HasMaxLength(50)
                .HasColumnName("categoriaArchivo");
            entity.Property(e => e.Contenido)
                .HasMaxLength(1)
                .HasColumnName("contenido");
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

        modelBuilder.Entity<CantidadCuota>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cantidad__3213E83F9FF8EBCC");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cuota).HasColumnName("cuota");
        });

        modelBuilder.Entity<CategoriasPersona>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3213E83F21E1F677");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Categoria)
                .HasMaxLength(30)
                .HasColumnName("categoria");
        });

        modelBuilder.Entity<Cementerio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cementer__3213E83F2FBC4407");

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
            entity.HasKey(e => e.Id).HasName("PK__Concepto__3213E83F14C051BF");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
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
            entity.HasKey(e => e.TramiteId).HasName("PK__Concesio__324535475D7C2C9D");

            entity.Property(e => e.TramiteId)
                .ValueGeneratedNever()
                .HasColumnName("tramiteId");
            entity.Property(e => e.CantidadAniosId).HasColumnName("cantidadAniosId");
            entity.Property(e => e.Concesion)
                .HasMaxLength(10)
                .HasColumnName("concesion");
            entity.Property(e => e.CuotaId).HasColumnName("cuotaId");
            entity.Property(e => e.FechaGeneracion)
                .HasColumnType("datetime")
                .HasColumnName("fechaGeneracion");
            entity.Property(e => e.PagoDescripcion)
                .HasMaxLength(200)
                .HasColumnName("pagoDescripcion");
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

            entity.HasOne(d => d.Tramite).WithOne(p => p.Concesione)
                .HasForeignKey<Concesione>(d => d.TramiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Concesiones_tramiteId_fk");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Concesiones)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("Concesiones_usuarioId_fk");
        });

        modelBuilder.Entity<EmpresasFunebre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Empresas__3213E83F0D3DB5E6");

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
            entity.HasKey(e => e.Id).HasName("PK__EstadosD__3213E83FB141CB84");

            entity.ToTable("EstadosDifunto");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .HasColumnName("estado");
        });

        modelBuilder.Entity<EstadosTramite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EstadosT__3213E83F46E12B2E");

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

        modelBuilder.Entity<HistorialEstadoTramite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Historia__3213E83F15E9AE86");

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

        modelBuilder.Entity<HistorialTitularesConcesione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Historia__3213E83F6BD64DA2");

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
            entity.HasKey(e => e.TramiteId).HasName("PK__Introduc__324535479FB599E0");

            entity.Property(e => e.TramiteId)
                .ValueGeneratedNever()
                .HasColumnName("tramiteId");
            entity.Property(e => e.DifuntoId).HasColumnName("difuntoId");
            entity.Property(e => e.EmpresaFunebreId).HasColumnName("empresaFunebreId");
            entity.Property(e => e.EstadoDifunto)
                .HasMaxLength(30)
                .HasColumnName("estadoDifunto");
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
            entity.HasKey(e => e.Id).HasName("PK__Notas__3213E83F355D7747");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Color)
                .HasMaxLength(16)
                .HasColumnName("color");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(1)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(1)
                .HasColumnName("nombre");
            entity.Property(e => e.TipoNotaId).HasColumnName("tipoNotaId");

            entity.HasOne(d => d.TipoNota).WithMany(p => p.Nota)
                .HasForeignKey(d => d.TipoNotaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Notas_tipoNotaId_fk");
        });

        modelBuilder.Entity<Parcela>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Parcelas__3213E83F8272F082");

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
            entity.HasKey(e => e.Id).HasName("PK__ParcelaD__3213E83F6D60374C");

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

        modelBuilder.Entity<Persona>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Personas__3213E83FA27B954E");

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
            entity.Property(e => e.DomicilioEnTirolesa).HasColumnName("domicilioEnTirolesa");
            entity.Property(e => e.EstadoDifuntoId).HasColumnName("estadoDifuntoId");
            entity.Property(e => e.FallecioEnTirolesa).HasColumnName("fallecioEnTirolesa");
            entity.Property(e => e.FechaDefuncion).HasColumnName("fechaDefuncion");
            entity.Property(e => e.FechaIngresoo)
                .HasColumnType("datetime")
                .HasColumnName("fechaIngresoo");
            entity.Property(e => e.FechaNacimiento).HasColumnName("fechaNacimiento");
            entity.Property(e => e.FechaRetiro)
                .HasColumnType("datetime")
                .HasColumnName("fechaRetiro");
            entity.Property(e => e.InformacionAdicional).HasColumnName("informacionAdicional");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.NroActa).HasColumnName("nroActa");
            entity.Property(e => e.NroAge).HasColumnName("nroAge");
            entity.Property(e => e.NroFolio).HasColumnName("nroFolio");
            entity.Property(e => e.NroSerie)
                .HasMaxLength(1)
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

        modelBuilder.Entity<PreciosTarifaria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PreciosT__3213E83F7E08FDEB");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AniosConcesionId).HasColumnName("aniosConcesionId");
            entity.Property(e => e.ConceptoTarifariaId).HasColumnName("conceptoTarifariaId");
            entity.Property(e => e.NroFila).HasColumnName("nroFila");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("precio");
            entity.Property(e => e.SeccionId).HasColumnName("seccionId");

            entity.HasOne(d => d.ConceptoTarifaria).WithMany(p => p.PreciosTarifaria)
                .HasForeignKey(d => d.ConceptoTarifariaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PreciosTarifarias_conceptoTarifariaId_fk");

            entity.HasOne(d => d.Seccion).WithMany(p => p.PreciosTarifaria)
                .HasForeignKey(d => d.SeccionId)
                .HasConstraintName("PreciosTarifarias_seccionId_fk");
        });

        modelBuilder.Entity<RolesUsuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RolesUsu__3213E83FECD04301");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Rol)
                .HasMaxLength(30)
                .HasColumnName("rol");
        });

        modelBuilder.Entity<Seccione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Seccione__3213E83FC76BAB90");

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
            entity.HasKey(e => e.Id).HasName("PK__Tareas__3213E83FA36BC5FD");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(1)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.NotaId).HasColumnName("notaId");
            entity.Property(e => e.TramiteId).HasColumnName("tramiteId");

            entity.HasOne(d => d.Nota).WithMany(p => p.Tareas)
                .HasForeignKey(d => d.NotaId)
                .HasConstraintName("Tareas_notaId_fk");

            entity.HasOne(d => d.Tramite).WithMany(p => p.Tareas)
                .HasForeignKey(d => d.TramiteId)
                .HasConstraintName("Tareas_tramiteId_fk");
        });

        modelBuilder.Entity<TemasTarifarium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TemasTar__3213E83F1450C1F4");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.Visibilidad)
                .HasDefaultValue(true)
                .HasColumnName("visibilidad");
        });

        modelBuilder.Entity<TipoNicho>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TipoNich__3213E83FCFCC900E");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<TipoNotum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TipoNota__3213E83F0F39236B");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion).HasMaxLength(30);
            entity.Property(e => e.Visibilidad).HasColumnName("visibilidad");
        });

        modelBuilder.Entity<TipoNumeracionParcela>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TipoNume__3213E83F1B1FC187");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TipoNumeracion)
                .HasMaxLength(30)
                .HasColumnName("tipoNumeracion");
        });

        modelBuilder.Entity<TipoPanteon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TipoPant__3213E83FFB5D83A5");

            entity.ToTable("TipoPanteon");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<TipoParcela>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TipoParc__3213E83F5E131AE3");

            entity.ToTable("TipoParcela");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Tipo)
                .HasMaxLength(30)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<TipoTramite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TipoTram__3213E83FC2F5F773");

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
            entity.HasKey(e => e.Id).HasName("PK__Tramites__3213E83F2005CF2A");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.EstadoActualId).HasColumnName("estadoActualId");
            entity.Property(e => e.FechaCreacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
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
            entity.HasKey(e => new { e.TramiteId, e.PersonaId }).HasName("PK__TramiteP__770E40CA1B32B3A2");

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

        modelBuilder.Entity<TramitesParcela>(entity =>
        {
            entity.HasKey(e => new { e.TramiteId, e.ParcelaId }).HasName("PK__Tramites__47EF37AD82619C0C");

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

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3213E83FB11BAC17");

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
