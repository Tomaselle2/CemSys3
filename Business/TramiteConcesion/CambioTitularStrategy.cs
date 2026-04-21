using CemSys3.DTOs.HistorialEstado;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.PlantillaTramite;
using CemSys3.DTOs.Tramite;
using CemSys3.DTOs.TramitesConcesion;
using CemSys3.DTOs.TramitesConcesion.CambioTitular;
using CemSys3.Enumerables;
using CemSys3.Helpers;
using CemSys3.Interfaces.HistorialEstados;
using CemSys3.Interfaces.Persona;
using CemSys3.Interfaces.PlantillaTramite;
using CemSys3.Interfaces.Tarea;
using CemSys3.Interfaces.Tramite;
using CemSys3.Interfaces.TramitesConcesion;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Business.TramiteConcesion
{
    public class CambioTitularStrategy : ITramiteStrategy,
    ITramiteCreateStrategy<CrearTramiteDTO, CambioTitularDTO>
    {
        //IGeneradorAutorizacionesStrategy
        private readonly IPlantillaTramite _plantillaService;
        private readonly IDocumentoTramiteService _documentoService;
        private readonly IPersona _personaService;
        private readonly AppDbContext _context;
        private readonly IHistorialEstados _historialEstadosService;
        private readonly ITareaPlantilla _tareaPlantilla;
        private readonly ITramite _tramiteService;


        public CambioTitularStrategy(
            IPlantillaTramite plantillaService,
            IDocumentoTramiteService documentoService,
            IPersona personaService,
            AppDbContext context,
            ITramite tramiteService, 
            IHistorialEstados historialEstadosService,
            ITareaPlantilla tareaPlantilla)
        {
            _plantillaService = plantillaService;
            _documentoService = documentoService;
            _personaService = personaService;
            _context = context;
            _tramiteService = tramiteService;
            _historialEstadosService = historialEstadosService;
            _tareaPlantilla = tareaPlantilla;
        }

       

        public async Task<int> CrearAsync(CrearTramiteDTO dto) //crea el tramite. 
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
                    TipoTramiteId = (int)TipoTramiteEnum.CambioTitular,
                    UsuarioId = dto.UsuarioId,
                    EstadoActualId = (int)EstadosCambioTitularEnum.Iniciado
                };

                int tramiteId = await _tramiteService.Add(tramite);
                await _context.SaveChangesAsync(); //guardar cambios antes de continuar

                //2- registrar Historial del tramite
                HistorialEstadosDTO historial = new HistorialEstadosDTO
                {
                    Fecha = tramite.FechaCreacion,
                    TramiteId = tramiteId,
                    EstadoTramiteId = (int)EstadosCambioTitularEnum.Iniciado
                };
                await _historialEstadosService.Add(historial);

                //3- registrar el tramite de cambio de titularidad
                Models.CambiosTitularidad cambiosTitularidad = new Models.CambiosTitularidad
                {
                    TramiteId = tramiteId,
                    ParcelaId = concesion.ParcelaId,
                    UsuarioId = dto.UsuarioId,
                    FechaCreacion = DateTime.Now,
                    InfoAdicional = string.Empty,
                    Visibilidad = true,
                    ConcesionId = dto.TramiteConcesionId
                };
                await _context.CambiosTitularidads.AddAsync(cambiosTitularidad);

                //4 - relacion de tramite con parcela
                await _historialEstadosService.VincularTramiteAParcela(tramiteId, concesion.ParcelaId);



                CambioTitularDTO dtoCambioTirular = new CambioTitularDTO();
                dtoCambioTirular.TramiteId = tramiteId;
                dtoCambioTirular.EstadoTramiteId = tramite.EstadoActualId;
                dtoCambioTirular.ParcelaId = concesion.ParcelaId;
                dtoCambioTirular.TipoParcela = concesion.TipoParcela;
                dtoCambioTirular.NombreSeccion = concesion.Parcela.Seccion.Nombre;
                dtoCambioTirular.NroParcela = concesion.Parcela.NroParcela;
                dtoCambioTirular.NroFila = concesion.Parcela.NroFila;
                dtoCambioTirular.NroConcesion = concesion.Concesion;

                //Traer Titulares en una sola consulta
                dtoCambioTirular.TitularesActuales = await _context.HistorialTitularesConcesiones
                    .Where(h => h.ConcesionId == dto.TramiteConcesionId && h.FechaFin == null)
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

                await _tareaPlantilla.CrearTareasPorTramite(tramiteId, (int)TipoTramiteEnum.CambioTitular);

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


        public Task FinalizarAsync(int tramiteId)
        {
            throw new NotImplementedException();
        }
        public Task<int> AvanzarEstadoAsync(int tramiteId, int nuevoEstado, int usuarioId)
        {
            throw new NotImplementedException();
        }

        public async Task GenerarDocumentosAsync(GeneraStrategyDTO dto)
        {
            var plantillas = await _plantillaService
                .ObtenerPorTipoTramiteAsync((int)TipoTramiteEnum.CambioTitular);

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

                var primerTitular = dto.TitularesActuales.FirstOrDefault();
                string sexoReferencia = primerTitular?.Sexo ?? "masculino"; // Valor por defecto

                foreach (var plantilla in plantillas)
                {
                    var variables = new Dictionary<string, string>
                {
                    { "Fecha", DateTime.Now.ToLongDateString() },
                    { "articuloTitularActual", sexoReferencia  == "masculino" ? "el" : "la"},
                    { "sr/sraTitularActual", sexoReferencia  == "masculino" ? "Sr." : "Sra."},
                    { "TitularesActuales", string.Join(", ", dto.TitularesActuales.Select(t => t.Apellido.ToUpper() + " " + t.Nombre.ToUpper())) },
                    { "DniTitularActual", string.Join(", ", dto.TitularesActuales.Select(t => StringExtensions.FormatearDni(t.Dni))) },
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
            Models.CambiosTitularidad cambioTitularidad = await _context.CambiosTitularidads.AsNoTracking()
                .Include(t => t.Tramite)
                .FirstOrDefaultAsync(ct => ct.TramiteId == tramiteId) ?? throw new Exception("Trámite de cambio de titularidad no encontrado.");

            Models.Concesione concesion = await _context.Concesiones.AsNoTracking()
                   .Include(c => c.Tramite)
                   .Include(c => c.Parcela)
                       .ThenInclude(p => p.Seccion)
                   .FirstOrDefaultAsync(c => c.TramiteId == cambioTitularidad.ConcesionId) ?? throw new Exception("Concesion no encontrada para inicar el trámite.");

            CambioTitularDTO dto = new CambioTitularDTO();
            dto.TramiteId = cambioTitularidad.TramiteId;
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


    }
}
