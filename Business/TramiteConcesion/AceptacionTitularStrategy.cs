using CemSys3.DTOs.HistorialEstado;
using CemSys3.DTOs.Nota;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.PlantillaTramite;
using CemSys3.DTOs.Tarea;
using CemSys3.DTOs.Tramite;
using CemSys3.DTOs.TramitesConcesion;
using CemSys3.DTOs.TramitesConcesion.CambioTitular;
using CemSys3.Enumerables;
using CemSys3.Helpers;
using CemSys3.Interfaces.HistorialEstados;
using CemSys3.Interfaces.Notas;
using CemSys3.Interfaces.Persona;
using CemSys3.Interfaces.PlantillaTramite;
using CemSys3.Interfaces.Tarea;
using CemSys3.Interfaces.Tramite;
using CemSys3.Interfaces.TramitesConcesion;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Business.TramiteConcesion
{
    public class AceptacionTitularStrategy : ITramiteStrategy,
    ITramiteCreateStrategy<CambioTitularDTO>
    {
        private readonly IPlantillaTramite _plantillaService;
        private readonly IDocumentoTramiteService _documentoService;
        private readonly IPersona _personaService;
        private readonly AppDbContext _context;
        private readonly IHistorialEstados _historialEstadosService;
        private readonly ITareaPlantilla _tareaPlantilla;
        private readonly ITramite _tramiteService;
        private readonly INotas _notasService;

        public AceptacionTitularStrategy(
            IPlantillaTramite plantillaService,
            IDocumentoTramiteService documentoService,
            IPersona personaService,
            AppDbContext context,
            ITramite tramiteService,
            IHistorialEstados historialEstadosService,
            ITareaPlantilla tareaPlantilla, INotas notasService)
        {
            _plantillaService = plantillaService;
            _documentoService = documentoService;
            _personaService = personaService;
            _context = context;
            _tramiteService = tramiteService;
            _historialEstadosService = historialEstadosService;
            _tareaPlantilla = tareaPlantilla;
            _notasService = notasService;
        }

        public Task<int> AvanzarEstadoAsync(int tramiteId, int nuevoEstado, int usuarioId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> CrearAsync(CrearTramiteDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                Models.Concesione concesion = await _context.Concesiones.AsNoTracking()
                   .Include(c => c.Tramite)
                   .Include(c => c.Parcela)
                       .ThenInclude(p => p.Seccion)
                   .FirstOrDefaultAsync(c => c.TramiteId == dto.TramiteConcesionId) ?? throw new Exception("Concesion no encontrada para inicar el trámite.");

                //1- registrar tramite
                TramiteDTO tramite = new TramiteDTO
                {
                    Visibilidad = true,
                    FechaCreacion = DateTime.Now,
                    TipoTramiteId = (int)TipoTramiteEnum.AceptacionTitular,
                    UsuarioId = dto.UsuarioId,
                    EstadoActualId = (int)EstadosTramiteEnum.Iniciado
                };

                int tramiteId = await _tramiteService.Add(tramite);
                await _context.SaveChangesAsync(); //guardar cambios antes de continuar

                //2- registrar Historial del tramite
                HistorialEstadosDTO historial = new HistorialEstadosDTO
                {
                    Fecha = tramite.FechaCreacion,
                    TramiteId = tramiteId,
                    EstadoTramiteId = (int)EstadosTramiteEnum.Iniciado
                };
                await _historialEstadosService.Add(historial);

                //3- registrar el tramite de cambio de titularidad
                Models.AceptacionTitularidad aceptacionTitularidad = new Models.AceptacionTitularidad
                {
                    TramiteId = tramiteId,
                    ParcelaId = concesion.ParcelaId,
                    UsuarioId = dto.UsuarioId,
                    FechaCreacion = DateTime.Now,
                    InfoAdicional = string.Empty,
                    Visibilidad = true,
                    ConcesionId = dto.TramiteConcesionId
                };
                await _context.AceptacionTitularidads.AddAsync(aceptacionTitularidad);

                //4 - relacion de tramite con parcela
                await _historialEstadosService.VincularTramiteAParcela(tramiteId, concesion.ParcelaId);

                await _tareaPlantilla.CrearTareasPorTramite(tramiteId, (int)TipoTramiteEnum.AceptacionTitular);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return tramiteId;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task FinalizarAsync(int tramiteId, int usuarioId)
        {
            Models.AceptacionTitularidad cambioTitularidad = await _context.AceptacionTitularidads
                 .Include(t => t.Tramite)
                 .FirstOrDefaultAsync(ct => ct.TramiteId == tramiteId) ?? throw new Exception("Trámite de aceptación de titularidad no encontrado.");

            Models.Concesione concesion = await _context.Concesiones
                   .FirstOrDefaultAsync(c => c.TramiteId == cambioTitularidad.ConcesionId) ?? throw new Exception("Concesion no encontrada.");

            Models.Tramite tramite = await _context.Tramites.FirstOrDefaultAsync(t => t.Id == tramiteId) ?? throw new Exception("Trámite no encontrado.");


            List<PersonaDTO> titularesNuevos = new();

            List<TitularesContratoDTO> nuevosTitulares = await _context.DocumentosTramites.Where(t => t.TramiteId == cambioTitularidad.TramiteId).Select(h => new TitularesContratoDTO
            {
                Id = h.Persona.Id,
            }).ToListAsync();

            foreach (var titular in nuevosTitulares)
            {
                titularesNuevos.Add(await _personaService.Get(titular.Id.Value));
                await _historialEstadosService.VincularTramiteAPersona(tramiteId, titular.Id.Value);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                //1-modificar los titulares de la concesion (agregar nuevos y cerrar los que ya no están)
                await ProcesarTitularesConHistorial(cambioTitularidad.ConcesionId.Value, titularesNuevos, concesion, null);

                //2- actualizar estado del tramite a finalizado
                tramite.EstadoActualId = (int)EstadosTramiteEnum.Finalizado;

                HistorialEstadosDTO historial = new HistorialEstadosDTO
                {
                    Fecha = DateTime.Now,
                    TramiteId = tramiteId,
                    EstadoTramiteId = (int)EstadosTramiteEnum.Finalizado
                };
                await _historialEstadosService.Add(historial);

                //3 generar la nota de recordatorio.
                string descripcionNota = $"\n● El {DateTime.Now:dd/MM/yyyy} se realizó la aceptación de titularidad (trámite {tramiteId})";
                string nombreNota = $"Para Program (concesión {concesion.Concesion?.ToString("D5") ?? "-----"})";
                string titularNota = $"El nuevo titular debe ser {titularesNuevos?[0].Apellido?.ToUpper()}, {titularesNuevos?[0].Nombre?.ToUpper()} con DNI {titularesNuevos?[0].Dni}";

                cambioTitularidad.FechaFinalizacion = DateTime.Now;
                tramite.FechaFinalizacion = DateTime.Now;

                await GenerarNotaRecordatorio(descripcionNota, nombreNota, titularNota, usuarioId);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task GenerarDocumentosAsync(GeneraStrategyDTO dto)
        {
            var plantillas = await _plantillaService
                .ObtenerPorTipoTramiteAsync((int)TipoTramiteEnum.AceptacionTitular);

            foreach (var nuevoTitular in dto.NuevosTitulares)
            {
                PersonaDTO persona = await _personaService.Get(nuevoTitular.Id.Value);

                persona.Nombre = nuevoTitular.Nombre;
                persona.Apellido = nuevoTitular.Apellido;
                persona.Domicilio = nuevoTitular.Domicilio;
                persona.Celular = nuevoTitular.Celular;
                persona.Correo = nuevoTitular.CorreoElectronico;

                int personaId = await _personaService.Update(persona);

                string difuntosFormateados = DifuntoFormatter.FormatearDifuntos(dto.Difuntos);

                foreach (var plantilla in plantillas)
                {
                    var variables = new Dictionary<string, string>
                    {
                        { "Fecha", DateTime.Now.ToLongDateString() },
                        { "Parcela", ParcelaFormatter.ObtenerParcela(dto.TipoParcela, dto.NroParcela, dto.NroFila, dto.NombreSeccion.ToUpper()) },
                        { "Difuntos", difuntosFormateados },
                        { "articuloNuevoTitular", persona.Sexo == "masculino" ? "al" : "a la"},
                        { "sr/sraNuevoTitular", persona.Sexo == "masculino" ? "Sr." : "Sra."},
                        { "NuevosTitulares", nuevoTitular.Apellido.ToUpper() + " " + nuevoTitular.Nombre.ToUpper() },
                        { "DniNuevosTitulares", StringExtensions.FormatearDni(nuevoTitular.Dni)  },

                    };

                    await _documentoService.CrearDesdePlantillaAsync(
                        plantilla.PlantillaId,
                        dto.TramiteId,
                        dto.UsuarioId,
                        nuevoTitular.Id,
                        dto.Parentesco,
                        variables
                    );
                }
            }
        }

        public async Task<CambioTitularDTO> ObtenerAsync(int tramiteId)
        {
            Models.AceptacionTitularidad cambioTitularidad = await _context.AceptacionTitularidads.AsNoTracking()
               .Include(t => t.Tramite)
               .FirstOrDefaultAsync(ct => ct.TramiteId == tramiteId) ?? throw new Exception("Trámite de aceptación de titularidad no encontrado.");

            Models.Concesione concesion = await _context.Concesiones.AsNoTracking()
                   .Include(c => c.Tramite)
                   .Include(c => c.Parcela)
                       .ThenInclude(p => p.Seccion)
                   .FirstOrDefaultAsync(c => c.TramiteId == cambioTitularidad.ConcesionId) ?? throw new Exception("Concesion no encontrada para inicar el trámite.");

            CambioTitularDTO dto = new CambioTitularDTO();
            dto.TramiteId = cambioTitularidad.TramiteId;
            dto.TipoTramiteId = cambioTitularidad.Tramite.TipoTramiteId;
            dto.EstadoTramiteId = cambioTitularidad.Tramite.EstadoActualId;
            dto.ParcelaId = cambioTitularidad.ParcelaId;
            dto.TipoParcela = concesion.TipoParcela;
            dto.NombreSeccion = concesion.Parcela.Seccion.Nombre;
            dto.NroParcela = concesion.Parcela.NroParcela;
            dto.NroFila = concesion.Parcela.NroFila;
            dto.NroConcesion = concesion.Concesion;
            dto.ConcesionId = concesion.TramiteId;

            dto.TitularesActuales = await _context.HistorialTitularesConcesiones
                    .Where(h => h.ConcesionId == cambioTitularidad.ConcesionId && h.FechaFin == null)
                    .Select(h => new TitularesContratoDTO
                    {
                        Id = h.Persona.Id,
                        Dni = h.Persona.Dni,
                        Nombre = h.Persona.Nombre,
                        Apellido = h.Persona.Apellido,
                        Sexo = h.Persona.Sexo,
                        Celular = h.Persona.Celular,
                        CorreoElectronico = h.Persona.Correo,
                        Domicilio = h.Persona.Domicilio
                    }).ToListAsync();

            dto.NuevosTitulares = await _context.DocumentosTramites.Where(t => t.TramiteId == cambioTitularidad.TramiteId).Select(h => new TitularesContratoDTO
            {
                Id = h.Persona.Id,
                Dni = h.Persona.Dni,
                Nombre = h.Persona.Nombre,
                Apellido = h.Persona.Apellido,
                Sexo = h.Persona.Sexo,
                Celular = h.Persona.Celular,
                CorreoElectronico = h.Persona.Correo,
                Domicilio = h.Persona.Domicilio
            }).ToListAsync();

            //consultar los difuntos relacionados a la parcela
            dto.Difuntos = await _context.ParcelaDifuntos
                .Where(p => p.ParcelaId == dto.ParcelaId && p.FechaRetiro == null)
                .Select(p => new DifuntoContratoDTO
                {
                    Id = p.Difunto.Id,
                    DNI = p.Difunto.Dni,
                    Nombre = p.Difunto.Nombre,
                    Apellido = p.Difunto.Apellido,
                    FechaIngreso = p.FechaIngreso,
                    EstadoDifuntoId = p.Difunto.EstadoDifuntoId
                }).ToListAsync();

            return dto;
        }

        private async Task ProcesarTitularesConHistorial(
            int tramiteId,
            List<PersonaDTO> titularesDTO, //nuevos
            Models.Concesione concesion,
            string? mensajeContrato = null)
        {
            // 1. Titulares actuales activos
            var titularesActuales = await _context.HistorialTitularesConcesiones
                .Where(p => p.ConcesionId == tramiteId && p.FechaFin == null)
                .ToListAsync();

            var idsActuales = titularesActuales.Select(t => t.PersonaId).ToList();

            var idsNuevos = new List<int>();

            if (titularesDTO == null || titularesDTO.Count == 0)
                return;

            foreach (var persona in titularesDTO)
            {
                int dni = int.Parse(persona.Dni);
                PersonaDTO personaDB;

                if (await _personaService.PersonaExiste(dni, persona.Sexo ?? ""))
                {
                    personaDB = await _personaService.GetByDNISexo(dni, persona.Sexo);

                    // actualizar datos
                    personaDB.Dni = persona.Dni?.PadLeft(8, '0');
                    personaDB.Nombre = persona.Nombre;
                    personaDB.Apellido = persona.Apellido;
                    personaDB.Sexo = persona.Sexo;
                    personaDB.Celular = persona.Celular;
                    personaDB.Correo = persona.Correo;
                    personaDB.Domicilio = persona.Domicilio;
                    personaDB.CategoriaPersonaId = (int)CategoriaPersonaEnum.Titular;

                    // 👉 MENSAJE SOLO SI VIENE (caso Update)
                    if (!string.IsNullOrEmpty(mensajeContrato))
                        personaDB.InformacionAdicional += mensajeContrato;

                    await _personaService.Update(personaDB);
                }
                else
                {
                    personaDB = new PersonaDTO
                    {
                        Dni = persona.Dni?.PadLeft(8, '0'),
                        Nombre = persona.Nombre,
                        Apellido = persona.Apellido,
                        Sexo = persona.Sexo,
                        Celular = persona.Celular,
                        Correo = persona.Correo,
                        Domicilio = persona.Domicilio,
                        CategoriaPersonaId = (int)CategoriaPersonaEnum.Titular,
                        InformacionAdicional = mensajeContrato ??
                            $"\n● El {DateTime.Now:dd/MM/yyyy} se agrega como titular en concesión ({concesion.Concesion?.ToString("D5") ?? "-----"})."
                    };

                    personaDB.Id = await _personaService.Add(personaDB);

                    concesion.InformacionAdicional +=
                        $"\n● El {DateTime.Now:dd/MM/yyyy} se agrega como titular a {personaDB.Apellido?.ToUpper()}, {personaDB.Nombre?.ToUpper()}.";
                }

                idsNuevos.Add(personaDB.Id);

                // 👉 NUEVO TITULAR REAL
                if (!idsActuales.Contains(personaDB.Id))
                {
                    if (!string.IsNullOrEmpty(mensajeContrato))
                        personaDB.InformacionAdicional += mensajeContrato;
                    else
                        personaDB.InformacionAdicional +=
                            $"\n● El {DateTime.Now:dd/MM/yyyy} se coloca como titular en concesión ({concesion.Concesion?.ToString("D5") ?? "-----"}).";

                    await _personaService.Update(personaDB);

                    concesion.InformacionAdicional +=
                        $"\n● El {DateTime.Now:dd/MM/yyyy} se agrega como nuevo titular a {personaDB.Apellido?.ToUpper()}, {personaDB.Nombre?.ToUpper()}.";

                    await _historialEstadosService.VincularTitularAConcesion(personaDB.Id, tramiteId);
                }

                await _historialEstadosService.VincularTramiteAPersona(tramiteId, personaDB.Id);
            }

            // 3. Cerrar titulares que ya no están
            foreach (var titularActual in titularesActuales)
            {
                if (!idsNuevos.Contains(titularActual.PersonaId.Value))
                {
                    titularActual.FechaFin = DateTime.Now;
                }
            }
        }

        private async Task GenerarNotaRecordatorio(string descripcionNota, string nombreNota, string titularNota, int usuarioId)
        {
            NotaDTO nota = new NotaDTO();
            nota.Nombre = nombreNota;
            nota.TipoNotaId = (int)TipoNotaEnum.Recordatorio;
            nota.Descripcion = descripcionNota;
            nota.Color = "#F5DADE";
            nota.Visibilidad = true;
            nota.EstadoId = (int)EstadosNotaEnum.NotaPendiente;
            nota.FechaCreacion = DateTime.Now;
            nota.UsurioId = usuarioId;
            nota.FechaFinRecordatorio = DateTime.Now.AddDays(10);
            nota.Tareas = new List<TareaDTO>
                {
                    new() { Descripcion = titularNota, Estado = false },
                };

            int tramiteNotaId = await _notasService.GenerarTramiteNota(usuarioId);
            await _notasService.GenerarNotaSinTransaccion(tramiteNotaId, nota);
        }


    }
}
