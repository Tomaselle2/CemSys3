using AspNetCoreGeneratedDocument;
using CemSys3.DTOs.Concesion;
using CemSys3.DTOs.Generics;
using CemSys3.DTOs.HistorialEstado;
using CemSys3.DTOs.Ingreso;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.Parcela;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.Tramite;
using CemSys3.Enumerables;
using CemSys3.Helpers.Enumerable;
using CemSys3.Interfaces.Concesion;
using CemSys3.Interfaces.HistorialEstados;
using CemSys3.Interfaces.Ingreso;
using CemSys3.Interfaces.Notas;
using CemSys3.Interfaces.Parcela;
using CemSys3.Interfaces.Persona;
using CemSys3.Interfaces.Tramite;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;
using static iText.Kernel.Pdf.Colorspace.PdfSpecialCs;

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
        private readonly IConcesion _concesionService;

        public IngresoService(AppDbContext context, ITramite tramiteService, 
            IHistorialEstados historialEstados, IParcela parcelaService,
            IPersona personaService, INotas notasService, IConcesion concesionService)
        {
            _context = context;
            _tramiteService = tramiteService;
            _historialEstadosService = historialEstados;
            _parcelaService = parcelaService;
            _personaService = personaService;
            _notaService = notasService;
            _concesionService = concesionService;
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

                //4- se registra la relacion (difunto con el tramite)\
                await _historialEstadosService.VincularTramiteAPersona(tramiteId, difuntoId);
                
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
                await _historialEstadosService.VincularTramiteAParcela(tramiteId, dto.ParcelaId);

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

                var parcela = await _context.Parcelas
                    .Include(p => p.Seccion)
                    .FirstOrDefaultAsync(p => p.Id == dto.ParcelaId) ?? throw new Exception("Parcela no encontrada");
                string ubicacion = "";

                if (parcela.TipoParcelaId == (int)TipoParcelaEnum.Nicho) //nicho
                {
                    ubicacion = $"Nicho {parcela.NroParcela.ToString()} Sección {parcela.Seccion.Nombre.ToUpper()} Fila {parcela.NroFila.ToString()}";
                }
                else if (parcela.TipoParcelaId == (int)TipoParcelaEnum.Fosa)//fosa
                {
                    ubicacion = $"Fosa {parcela.NroParcela.ToString()} Sección {parcela.Seccion.Nombre.ToUpper()}";
                }
                else if (parcela.TipoParcelaId == (int)TipoParcelaEnum.Panteon) //panteon
                {
                    ubicacion = $"Lote {parcela.NroParcela.ToString()} Sección {parcela.Seccion.Nombre.ToUpper()}";
                }

                if (parcela.CantidadDifuntos == 0)
                {
                    parcelaDifunto.Difunto.InformacionAdicional += $"\n● El {DateTime.Now.ToString("dd/MM/yyyy")} en {ubicacion} se genera concesión en estado '{EnumHelper.GetDisplayNameByValue<EstadosConcesionEnum>((int)EstadosConcesionEnum.SinContrato)}'.";
                }

                //8- se debe sumar en 1 la cantidad de difuntos en la parcela
                await _parcelaService.AumentarDifunto(ingreso.ParcelaId);

                //9- Vincular la nota con el ingreso
                await _notaService.VincularNotaConIngreso(dto.NotaId, tramiteId);

                //10- se inicia el contrato de concesion en estado "Sin Contrato" solo si es nicho o fosa

                bool existeConcesion = await _context.Concesiones
                    .AnyAsync(c => c.ParcelaId == ingreso.ParcelaId && c.Visibilidad == true);


                //11- informacion adicional de ingreso (parcela)
                parcelaDifunto.Parcela.InformacionAdicional += $"\n● El {dto.FechaIngreso?.ToString("dd/MM/yyyy HH:mm")} se realizó el ingreso del difunto {difunto.Apellido?.ToUpper()}, {difunto.Nombre?.ToUpper()} en estado {EnumHelper.GetDisplayNameByValue<EstadoDifuntoEnum>(dto.EstadoDifuntoId)}.";

                //12- informacion adicional de ingreso (difunto)
                parcelaDifunto.Difunto.InformacionAdicional += $"\n● El {dto.FechaIngreso?.ToString("dd/MM/yyyy HH:mm")} se realizó el ingreso en {ubicacion} en estado {EnumHelper.GetDisplayNameByValue<EstadoDifuntoEnum>(dto.EstadoDifuntoId)}.";


                if (!existeConcesion && ingreso.Parcela.TipoParcelaId != (int)TipoParcelaEnum.Panteon)
                {
                    ConcesionDTO concesion = new ConcesionDTO();
                    concesion.ParcelaId = ingreso.ParcelaId;
                    concesion.TipoParcela = EnumHelper.GetDisplayNameByValue<TipoParcelaEnum>(ingreso.Parcela.TipoParcelaId ?? 0);
                    concesion.UsuarioId = ingreso.UsuarioId;
                    concesion.EstadoTramiteId = (int)EstadosConcesionEnum.SinContrato;
                    concesion.MensajeParcela = $"\n● El {DateTime.Now.ToString("dd/MM/yyyy")} para difunto {difunto.Apellido?.ToUpper()}, {difunto.Nombre?.ToUpper()} se genera concesión en estado '{EnumHelper.GetDisplayNameByValue<EstadosConcesionEnum>((int)EstadosConcesionEnum.SinContrato)}'.";
                    concesion.InformacionAdicional = $"\n● El {DateTime.Now.ToString("dd/MM/yyyy")} en {ubicacion} se genera concesión en estado '{EnumHelper.GetDisplayNameByValue<EstadosConcesionEnum>((int)EstadosConcesionEnum.SinContrato)}'.";
                    GenericResultDTO resultadoConcesion = await _concesionService.Add(concesion);
                }

                if (!existeConcesion && ingreso.Parcela.TipoParcelaId == (int)TipoParcelaEnum.Panteon)
                {
                    //se crea la concesion para cada panteon registrado, con estado vigente
                    ConcesionDTO concesion = new ConcesionDTO();
                    concesion.Visibilidad = true;
                    concesion.ParcelaId = ingreso.Parcela.Id;
                    concesion.TipoParcela = EnumHelper.GetDisplayNameByValue<TipoParcelaEnum>(ingreso.Parcela.TipoParcelaId ?? 0);
                    concesion.UsuarioId = ingreso.UsuarioId;
                    concesion.EstadoTramiteId = (int)EstadosConcesionEnum.Vigente;
                    GenericResultDTO resultadoConcesion = await _concesionService.Add(concesion);
                }
                

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
                ingreso.InformacionAdicional += $"\nDetalle del cobro: {cobroIngreso}";
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

        public async Task<PaginadoResponse<ListadoIngresosDTO>> GetAllPaginadoIngresos(DateOnly? fechaDesde, DateOnly? fechaHasta, int pagina = 1, int porPagina = 10, int filtro = 0)
        {
            PaginadoResponse<ListadoIngresosDTO> resultado = new PaginadoResponse<ListadoIngresosDTO>();

            var query = _context.Introducciones.Include(d=> d.Difunto).Include(t=> t.Tramite).Include(p => p.Parcela).AsQueryable();

            // Filtro por estado del tramite
            switch (filtro)
            {
                case 1: //registrados
                    query = query.Where(e => e.Tramite.EstadoActualId == (int)EstadosIngresoEnum.IngresoRegistrado);
                    break;
                case 2: //finalizados
                    query = query.Where(e => e.Tramite.EstadoActualId == (int)EstadosIngresoEnum.IngresoFinalizado);
                    break;
                case 0: //todos
                default:
                    // No aplicar filtro
                    break;
            }



            // Aplicar filtros de fecha si existen
            if (fechaDesde.HasValue)
            {
                DateTime _fechaDesde = fechaDesde.Value.ToDateTime(TimeOnly.MinValue);
               
                query = query.Where(x => x.FechaIngreso >= _fechaDesde);
            }

            if (fechaHasta.HasValue)
            {
                DateTime _fechaHasta = fechaHasta.Value.ToDateTime(TimeOnly.MinValue);

                // Añadir un día para incluir todo el día hasta
                query = query.Where(x => x.FechaIngreso < _fechaHasta.AddDays(1));
            }

            // Total de registros
            var total = await query.CountAsync();

            // Paginación
            resultado.Paginacion.TotalPaginas = (int)Math.Ceiling(total / (double)porPagina);
            resultado.Paginacion.PaginaActual = Math.Max(1, Math.Min(pagina, resultado.Paginacion.TotalPaginas));
            resultado.Paginacion.RegistrosPorPagina = porPagina;
            resultado.Paginacion.Accion = "ListadoIngresos";
            resultado.Paginacion.Controlador = "Ingreso";
            resultado.Paginacion.TotalRegistros = total;

            // Obtener datos paginados
            resultado.Items = await query
                .OrderByDescending(e => e.FechaIngreso)
                .Skip((resultado.Paginacion.PaginaActual - 1) * porPagina)
                .Take(porPagina)
                .Select(e => new ListadoIngresosDTO
                {
                    TramiteId = e.TramiteId,
                    EstadoTramiteId = e.Tramite.EstadoActualId,
                    NroParcela = e.Parcela.NroParcela,
                    NroFila = e.Parcela.NroFila,
                    TipoParcelaId = e.Parcela.TipoParcelaId ?? 0,
                    NombreSeccion = e.Parcela.Seccion.Nombre.ToUpper(),
                    NombreDifunto = e.Difunto.Nombre ?? "----",
                    ApellidoDifunto = e.Difunto.Apellido ?? "----",
                    FechaIngreso = e.FechaIngreso
                }).ToListAsync();

            return resultado;
        }
    }
}
