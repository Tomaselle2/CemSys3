using AspNetCoreGeneratedDocument;
using CemSys3.DTOs.Generics;
using CemSys3.DTOs.HistorialEstado;
using CemSys3.DTOs.Ingreso;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.Tramite;
using CemSys3.Enumerables;
using CemSys3.Interfaces.HistorialEstados;
using CemSys3.Interfaces.Ingreso;
using CemSys3.Interfaces.Notas;
using CemSys3.Interfaces.Parcela;
using CemSys3.Interfaces.Persona;
using CemSys3.Interfaces.Tramite;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Business.Ingreso
{
    public class IngresoService : IIngreso
    {
        private readonly AppDbContext _context;
        private readonly ITramite _tramiteService;
        private readonly IHistorialEstados _historialEstadosService;
        private readonly IPersona _personaService;
        private readonly IParcela _parcelaService;
        private readonly INotas _notaService;

        public IngresoService(AppDbContext context, ITramite tramiteService, 
            IHistorialEstados historialEstados, IParcela parcelaService,
            IPersona personaService, INotas notasService)
        {
            _context = context;
            _tramiteService = tramiteService;
            _historialEstadosService = historialEstados;
            _parcelaService = parcelaService;
            _personaService = personaService;
            _notaService = notasService;
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

                //8- se debe sumar en 1 la cantidad de difuntos en la parcela
                await _parcelaService.AumentarDifunto(ingreso.ParcelaId);

                //9- Vincular la nota con el ingreso
                await _notaService.VincularNotaConIngreso(dto.NotaId, tramiteId);

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

        public async Task FinalizarIngreso(int ingresoId, string cobroIngreso, string cobroApertura)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                //se registrar el historial
                HistorialEstadosDTO historial = new HistorialEstadosDTO
                {
                    Fecha = DateTime.Now,
                    TramiteId = ingresoId,
                    EstadoTramiteId = (int)EstadosIngresoEnum.IngresoFinalizado
                };
                await _historialEstadosService.Add(historial);

                //se modifica el estadoActual en la tabla tramite
                Models.Tramite tramite = await _context.Tramites.FindAsync(ingresoId) ?? throw new Exception("Trámite no encontrado");
                tramite.EstadoActualId = (int)EstadosIngresoEnum.IngresoFinalizado;

                //Se suma el cobroIngreso en el infoAdicional del tramite Introduccion
                Models.Introduccione ingreso = await _context.Introducciones.FindAsync(ingresoId) ?? throw new Exception("Ingreso no encontrado");
                ingreso.InformacionAdicional += $"\nDetalle el cobro: {cobroIngreso}";
                ingreso.InformacionAdicional += $"\nDetalle de la apertura: {cobroApertura}";

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ResumenIngresoDTO> Get(int ingresoId)
        {
            ResumenIngresoDTO ingreso = await _context.Introducciones.Where(i => i.TramiteId == ingresoId).Select(s => new ResumenIngresoDTO
            {
                TramiteId = s.TramiteId,
                Visibilidad = s.Visibilidad,
                FechaIngreso = s.FechaIngreso,
                UsuarioLogueadoId = s.UsuarioId,
                UsuarioLogueadoNombre = s.Usuario.Nombre + " " + s.Usuario.Apellido,
                EmpleadoIngresoId = s.UsuarioId,
                EmpleadoIngresoNombre = s.Usuario.Nombre + " " + s.Usuario.Apellido,
                EmpresaFunebreId = s.EmpresaFunebreId,
                EmpresaFunebreNombre = s.EmpresaFunebre != null ? s.EmpresaFunebre.Nombre : null,
                ParcelaId = s.ParcelaId,
                NroParcela = s.Parcela.NroParcela,
                SeccionNombre = s.Parcela.Seccion.Nombre,
                NroFila = s.Parcela.NroFila,
                DifuntoId = s.DifuntoId,
                EstadoDifuntoId = s.EstadoDifuntoId,
                EstadoDifuntoNombre = s.EstadoDifunto.Estado,
                InformacionAdicional = s.InformacionAdicional,
                TipoParcelaId = s.Parcela.TipoParcelaId ?? 0,
                EstadoActualId = s.Tramite.EstadoActualId,
                Difunto = new PersonaDTO
                {
                    Nombre = s.Difunto.Nombre,
                    Apellido = s.Difunto.Apellido,
                    Dni = s.Difunto.Dni,
                    FechaNacimiento = s.Difunto.FechaNacimiento,
                    FechaDefuncion = s.Difunto.FechaDefuncion,
                    InformacionAdicional = s.Difunto.InformacionAdicional,
                    Sexo = s.Difunto.Sexo,
                    NroActa = s.Difunto.NroActa,
                    NroFolio = s.Difunto.NroFolio,
                    NroTomo = s.Difunto.NroTomo,
                    NroSerie = s.Difunto.NroSerie,
                    NroAge = s.Difunto.NroAge,
                    EstadoDifuntoId = s.Difunto.EstadoDifuntoId
                }
            }).FirstOrDefaultAsync() ?? throw new Exception("No se encontro el ingreso");

            //obtener la nota asociada
            ingreso.Nota = await _notaService.GetNotaIngreso(ingresoId);

            return ingreso;
        }

    }
}
