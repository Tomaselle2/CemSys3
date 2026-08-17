using CemSys3.DTOs.CargaDifunto;
using CemSys3.DTOs.Concesion;
using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Persona;
using CemSys3.Enumerables;
using CemSys3.Helpers.Enumerable;
using CemSys3.Interfaces.CargaDifunto;
using CemSys3.Interfaces.Concesion;
using CemSys3.Interfaces.HistorialEstados;
using CemSys3.Interfaces.Parcela;
using CemSys3.Interfaces.Persona;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Cms;

namespace CemSys3.Business.CargaDifunto
{
    public class CargaDifuntoService : ICargaDifunto
    {
        private readonly AppDbContext _context;
        private readonly IHistorialEstados _historialEstadosService;
        private readonly IParcela _parcelaService;
        private readonly IConcesion _concesionService;
        private readonly IPersona _personaService;
        public CargaDifuntoService(AppDbContext context,
            IHistorialEstados historialEstados, IParcela parcelaService,
            IConcesion concesionService, IPersona personaService)
        {
            _context = context;
            _historialEstadosService = historialEstados;
            _parcelaService = parcelaService;
            _concesionService = concesionService;
            _personaService = personaService;
        }

        public async Task<GenericResultDTO> Add(CargaDifuntoDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                int difuntoId;
                PersonaDTO difunto;

                if (dto.PersonaExistenteId.HasValue)
                {
                    // Reutilizamos una persona existente (titular fallecido o difunto retirado que reingresa)
                    difuntoId = dto.PersonaExistenteId.Value;
                    await _personaService.CambiarCategoria(difuntoId, (int)CategoriaPersonaEnum.Fallecido);

                    difunto = new PersonaDTO
                    {
                        Id = difuntoId,
                        Nombre = dto.Difunto.Nombre?.Trim(),
                        Apellido = dto.Difunto.Apellido?.Trim(),
                        Dni = dto.Difunto.Dni,
                        FechaNacimiento = dto.Difunto.FechaNacimiento,
                        FechaDefuncion = dto.Difunto.FechaDefuncion,
                        InformacionAdicional = "\n" + dto.Difunto.InformacionAdicional,
                        Sexo = dto.Difunto.Sexo,
                        NroActa = dto.Difunto.NroActa,
                        NroFolio = dto.Difunto.NroFolio,
                        NroTomo = dto.Difunto.NroTomo,
                        NroSerie = dto.Difunto.NroSerie,
                        NroAge = dto.Difunto.NroAge,
                        EstadoDifuntoId = dto.Difunto.EstadoDifuntoId,
                        FechaIngreso = dto.Difunto.FechaIngreso ?? DateTime.Now
                    };

                    await _personaService.UpdateDatosIngresoTitularFallecido(difunto);
                }
                else
                {
                    //3- se registra el difunto
                    difunto = new PersonaDTO
                    {
                        Nombre = dto.Difunto.Nombre?.Trim().ToLower(),
                        Apellido = dto.Difunto.Apellido?.Trim().ToLower(),
                        Dni = dto.Difunto.Dni,
                        Visibilidad = true,
                        FechaNacimiento = dto.Difunto.FechaNacimiento,
                        FechaDefuncion = dto.Difunto.FechaDefuncion,
                        InformacionAdicional = "\n" + dto.Difunto.InformacionAdicional,
                        Sexo = dto.Difunto.Sexo,
                        NroActa = dto.Difunto.NroActa,
                        NroFolio = dto.Difunto.NroFolio,
                        NroTomo = dto.Difunto.NroTomo,
                        NroSerie = dto.Difunto.NroSerie,
                        NroAge = dto.Difunto.NroAge,
                        EstadoDifuntoId = dto.Difunto.EstadoDifuntoId,
                        CategoriaPersonaId = (int)CategoriaPersonaEnum.Fallecido,
                        FechaIngreso = dto.Difunto.FechaIngreso ?? DateTime.Now
                    };
                    difuntoId = await _personaService.Add(difunto);
                }

                //5- se registra la relacion (parcela con difunto)
                ParcelaDifunto parcelaDifunto = new ParcelaDifunto
                {
                    ParcelaId = dto.ParcelaId,
                    DifuntoId = difuntoId,
                    FechaIngreso = dto.Difunto.FechaIngreso ?? DateTime.Now,
                    TramiteIngresoId = null
                };
                _context.ParcelaDifuntos.Add(parcelaDifunto);
                await _context.SaveChangesAsync();

                var parcela = await _context.Parcelas
                    .Include(p => p.Seccion)
                    .FirstOrDefaultAsync(p => p.Id == dto.ParcelaId) ?? throw new Exception("Parcela no encontrada");
                string ubicacion = "";

                if (parcela.TipoParcelaId == (int)TipoParcelaEnum.Nicho)
                {
                    ubicacion = $"Nicho {parcela.NroParcela.ToString()} Sección {parcela.Seccion.Nombre.ToUpper()} Fila {parcela.NroFila.ToString()}";
                }
                else if (parcela.TipoParcelaId == (int)TipoParcelaEnum.Fosa)
                {
                    ubicacion = $"Fosa {parcela.NroParcela.ToString()} Sección {parcela.Seccion.Nombre.ToUpper()}";
                }
                else if (parcela.TipoParcelaId == (int)TipoParcelaEnum.Panteon)
                {
                    ubicacion = $"Lote {parcela.NroParcela.ToString()} Sección {parcela.Seccion.Nombre.ToUpper()}";
                }

                //8- se debe sumar en 1 la cantidad de difuntos en la parcela
                await _parcelaService.AumentarDifunto(dto.ParcelaId);

                //10- se inicia el contrato de concesion en estado "Sin Contrato" solo si es nicho o fosa
                bool existeConcesion = await _context.Concesiones
                    .AnyAsync(c => c.ParcelaId == dto.ParcelaId && c.Visibilidad == true && c.FechaFin == null);

                if (!existeConcesion && parcela.TipoParcelaId != (int)TipoParcelaEnum.Panteon)
                {
                    ConcesionDTO concesion = new ConcesionDTO();
                    concesion.ParcelaId = parcela.Id;
                    concesion.TipoParcela = EnumHelper.GetDisplayNameByValue<TipoParcelaEnum>(parcela.TipoParcelaId ?? 0);
                    concesion.UsuarioId = dto.UsuarioLogueadoId;
                    concesion.EstadoTramiteId = (int)EstadosConcesionEnum.SinContrato;
                    concesion.MensajeParcela = $"\n● El {DateTime.Now.ToString("dd/MM/yyyy")} para difunto {difunto.Apellido?.ToUpper()}, {difunto.Nombre?.ToUpper()} se genera concesión en estado '{EnumHelper.GetDisplayNameByValue<EstadosConcesionEnum>((int)EstadosConcesionEnum.SinContrato)}'.";
                    concesion.FechaInicio = difunto.FechaIngreso ?? DateTime.Now;
                    concesion.InformacionAdicional = $"\n● El {DateTime.Now.ToString("dd/MM/yyyy")} en {ubicacion} se genera concesión en estado '{EnumHelper.GetDisplayNameByValue<EstadosConcesionEnum>((int)EstadosConcesionEnum.SinContrato)}'.";
                    GenericResultDTO resultadoConcesion = await _concesionService.Add(concesion);
                }

                if (!existeConcesion && parcela.TipoParcelaId == (int)TipoParcelaEnum.Panteon)
                {
                    ConcesionDTO concesion = new ConcesionDTO();
                    concesion.Visibilidad = true;
                    concesion.ParcelaId = parcela.Id;
                    concesion.TipoParcela = EnumHelper.GetDisplayNameByValue<TipoParcelaEnum>(parcela.TipoParcelaId ?? 0);
                    concesion.UsuarioId = dto.UsuarioLogueadoId;
                    concesion.FechaInicio = difunto.FechaIngreso ?? DateTime.Now;
                    concesion.EstadoTramiteId = (int)EstadosConcesionEnum.Vigente;
                    concesion.Vencimiento = new DateOnly(9999,12,30);
                    GenericResultDTO resultadoConcesion = await _concesionService.Add(concesion);
                }

                Models.Concesione concesionBD = await _context.Concesiones
                   .FirstOrDefaultAsync(c => c.ParcelaId == parcela.Id && c.FechaFin == null) ?? throw new Exception("Concesion no encontrada.");

                await _historialEstadosService.VincularTramiteAPersona(concesionBD.TramiteId, difuntoId);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new GenericResultDTO
                {
                    Success = true,
                    Message = "Carga registrada con éxito.",
                    Id = parcela.Id
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
