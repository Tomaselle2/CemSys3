using CemSys3.DTOs.Generics;
using CemSys3.DTOs.HistorialEstado;
using CemSys3.DTOs.Ingreso;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.Tramite;
using CemSys3.Enumerables;
using CemSys3.Helpers.Enumerable;
using CemSys3.Interfaces.HistorialEstados;
using CemSys3.Interfaces.Ingreso;
using CemSys3.Interfaces.Persona;
using CemSys3.Interfaces.Tramite;
using CemSys3.Models;

namespace CemSys3.Business.Ingreso
{
    public class IngresoService : IIngreso
    {
        private readonly AppDbContext _context;
        private readonly ITramite _tramiteService;
        private readonly IHistorialEstados _historialEstadosService;
        private readonly IPersona _personaService;
        public IngresoService(AppDbContext context, ITramite tramiteService, IHistorialEstados historialEstados,
            IPersona personaService)
        {
            _context = context;
            _tramiteService = tramiteService;
            _historialEstadosService = historialEstados;
            _personaService = personaService;
        }

        public async Task<GenericResultDTO> Add(IngresoDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                //1- registrar tramite
                TramiteDTO tramite = new TramiteDTO
                {
                    Visibilidad = true,
                    FechaCreacion = DateTime.Now,
                    TipoTramiteId = (int)TipoTramiteEnum.Ingreso,
                    UsuarioId = dto.UsuarioLogueadoId,
                    EstadoActualId = (int)EstadosIngresoEnum.IngresoRegistrado //registrado
                };
                int tramiteId = await _tramiteService.Add(tramite);
                await _context.SaveChangesAsync(); //guardar cambios antes de continuar

                //2- registrar Historial del tramite
                HistorialEstadosDTO historial = new HistorialEstadosDTO
                {
                    Fecha = DateTime.Now,
                    TramiteId = tramiteId,
                    EstadoTramiteId = (int)EstadosIngresoEnum.IngresoRegistrado
                };
                await _historialEstadosService.Add(historial);

                //3- se registra el difunto
                PersonaDTO difunto = new PersonaDTO
                {
                    Nombre = dto.Difunto.Nombre?.Trim().ToLower(),
                    Apellido = dto.Difunto.Apellido?.Trim().ToLower(),
                    Dni = dto.Difunto.Dni,
                    Visibilidad = true,
                    FechaNacimiento = dto.Difunto.FechaNacimiento,
                    FechaDefuncion  = dto.Difunto.FechaDefuncion,
                    InformacionAdicional = dto.Difunto.InformacionAdicional,
                    Sexo = dto.Difunto.Sexo,
                    NroActa = dto.Difunto.NroActa,
                    NroFolio = dto.Difunto.NroFolio,
                    NroTomo = dto.Difunto.NroTomo,
                    NroSerie = dto.Difunto.NroSerie,
                    NroAge = dto.Difunto.NroAge,
                    EstadoDifuntoId = dto.Difunto.EstadoDifuntoId,
                    CategoriaPersonaId = (int)CategoriaPersonaEnum.Fallecido
                };
                int difuntoId = await _personaService.Add(difunto);

                //4- se registra la relacion (difunto con el tramite)
                TramitePersona tramitePersona = new TramitePersona
                {
                    PersonaId = difuntoId,
                    TramiteId = tramiteId,
                    FechaRegistro = DateTime.Now
                };
                _context.TramitePersonas.Add(tramitePersona);

                //5- se registra la relacion (parcela con difunto)
                ParcelaDifunto parcelaDifunto = new ParcelaDifunto
                {
                    ParcelaId = dto.ParcelaId,
                    DifuntoId = difuntoId,
                    FechaIngreso = dto.FechaIngreso,
                    TramiteIngresoId = tramiteId
                };
                _context.ParcelaDifuntos.Add(parcelaDifunto);

                //6- se registra la relacion (trámite con parcela)
                TramitesParcela tramiteParcela = new TramitesParcela
                {
                    TramiteId = tramiteId,
                    ParcelaId = dto.ParcelaId,
                    FechaRegistro = DateTime.Now
                };
                _context.TramitesParcelas.Add(tramiteParcela);

                //7- se registra el ingreso
                Introduccione ingreso = new Introduccione
                {
                    TramiteId = tramiteId,
                    Visibilidad = true,
                    FechaIngreso = dto.FechaIngreso,
                    UsuarioId = dto.EmpleadoIngresoId,
                    EmpresaFunebreId = dto.EmpresaFunebreId,
                    ParcelaId = dto.ParcelaId,
                    DifuntoId = difuntoId,
                    EstadoDifuntoId = dto.EstadoDifuntoId,
                    InformacionAdicional = dto.InformacionAdicional
                };
                _context.Introducciones.Add(ingreso);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new GenericResultDTO
                {
                    Success = true,
                    Message = "Ingreso registrado con éxito.",
                    Id = tramiteId
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
