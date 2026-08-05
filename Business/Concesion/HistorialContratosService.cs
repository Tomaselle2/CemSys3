using CemSys3.DTOs.Concesion;
using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.Persona;
using CemSys3.Interfaces.Concesion;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Business.Concesion
{
    public class HistorialContratosService : IHistorialContratosService
    {
        private readonly AppDbContext _context;
        public HistorialContratosService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GenericResultDTO> Add(HistorialContratoDTO dto)
        {
            try
            {
                var historial = new Models.HistorialContratosConcesion
                {
                    TramiteId = dto.TramiteId,
                    Concesion = dto.Concesion,
                    ParcelaId = dto.ParcelaId,
                    FechaContrato = dto.FechaContrato == default ? DateTime.Now : dto.FechaContrato,
                    EsRenovacion = dto.EsRenovacion,
                    UsuarioId = dto.UsuarioId,
                    Visibilidad = true
                };

                _context.HistorialContratosConcesions.Add(historial);
                await _context.SaveChangesAsync(); // necesito el Id antes de insertar los difuntos

                if (dto.DifuntosIds != null && dto.DifuntosIds.Count > 0)
                {
                    foreach (var difuntoId in dto.DifuntosIds)
                    {
                        _context.HistorialContratosConcesionDifuntos.Add(new Models.HistorialContratosConcesionDifunto
                        {
                            HistorialContratoId = historial.Id,
                            DifuntoId = difuntoId
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                return new GenericResultDTO
                {
                    Success = true,
                    Message = "Historial de contrato guardado con éxito.",
                    Id = historial.Id
                };
            }
            catch (Exception ex)
            {
                return new GenericResultDTO
                {
                    Success = false,
                    Message = $"Error al guardar historial de contrato: {ex.Message}"
                };
            }
        }

        public async Task<PaginadoResponse<HistorialContratoTablaDTO>> GetAllPaginado(
      int pagina = 1,
      int porPagina = 10,
      DateOnly? fechaDesde = null,
      DateOnly? fechaHasta = null)
        {
            var resultado = new PaginadoResponse<HistorialContratoTablaDTO>();

            var query = _context.HistorialContratosConcesions
                .Where(h => h.Visibilidad)
                .AsQueryable().AsNoTracking();

            if (fechaDesde.HasValue)
                query = query.Where(h => h.FechaContrato >= fechaDesde.Value.ToDateTime(TimeOnly.MinValue));

            if (fechaHasta.HasValue)
                query = query.Where(h => h.FechaContrato < fechaHasta.Value.AddDays(1).ToDateTime(TimeOnly.MinValue));

            var total = await query.CountAsync();
            resultado.Paginacion.TotalPaginas = (int)Math.Ceiling(total / (double)porPagina);
            resultado.Paginacion.PaginaActual = Math.Max(1, Math.Min(pagina, Math.Max(1, resultado.Paginacion.TotalPaginas)));
            resultado.Paginacion.RegistrosPorPagina = porPagina;
            resultado.Paginacion.Accion = "TablaHistorialContratos";
            resultado.Paginacion.Controlador = "Concesion";
            resultado.Paginacion.TotalRegistros = total;

            var historialPagina = await query
                .OrderByDescending(h => h.FechaContrato)
                .Skip((resultado.Paginacion.PaginaActual - 1) * porPagina)
                .Take(porPagina)
                .Select(h => new
                {
                    h.Id,
                    h.TramiteId,
                    h.Concesion,
                    h.FechaContrato,
                    h.EsRenovacion,
                    NombreSeccion = h.Parcela.Seccion.Nombre,
                    h.Parcela.NroParcela,
                    h.Parcela.NroFila,
                    TipoParcelaId = h.Parcela.TipoParcelaId
                })
                .ToListAsync();

            var historialIds = historialPagina.Select(h => h.Id).ToList();

            var difuntos = await _context.HistorialContratosConcesionDifuntos
                .Where(d => historialIds.Contains(d.HistorialContratoId))
                .Select(d => new
                {
                    d.HistorialContratoId,
                    Persona = new PersonaTablaGeneral
                    {
                        Nombre = d.Difunto.Nombre ?? "",
                        Apellido = d.Difunto.Apellido ?? ""
                    }
                })
                .ToListAsync();

            resultado.Items = historialPagina.Select(h => new HistorialContratoTablaDTO
            {
                Id = h.Id,
                TramiteId = h.TramiteId,
                Concesion = h.Concesion,
                FechaContrato = h.FechaContrato,
                EsRenovacion = h.EsRenovacion,
                TipoParcelaId = h.TipoParcelaId ?? 0,
                NombreSeccion = h.NombreSeccion,
                NroParcela = h.NroParcela,
                NroFila = h.NroFila,
                Difuntos = difuntos.Where(d => d.HistorialContratoId == h.Id).Select(d => d.Persona).ToList()
            }).ToList();

            return resultado;
        }

        public async Task<List<HistorialContratoTablaDTO>> GetAllParaExportar(
            DateOnly? fechaDesde = null,
            DateOnly? fechaHasta = null)
        {
            var query = _context.HistorialContratosConcesions
                .Where(h => h.Visibilidad)
                .AsQueryable().AsNoTracking();

            if (fechaDesde.HasValue)
                query = query.Where(h => h.FechaContrato >= fechaDesde.Value.ToDateTime(TimeOnly.MinValue));

            if (fechaHasta.HasValue)
                query = query.Where(h => h.FechaContrato < fechaHasta.Value.AddDays(1).ToDateTime(TimeOnly.MinValue));

            var datos = await query
                .OrderByDescending(h => h.FechaContrato)
                .Select(h => new
                {
                    h.Id,
                    h.TramiteId,
                    h.Concesion,
                    h.FechaContrato,
                    h.EsRenovacion,
                    NombreSeccion = h.Parcela.Seccion.Nombre,
                    h.Parcela.NroParcela,
                    h.Parcela.NroFila,
                    TipoParcelaId = h.Parcela.TipoParcelaId
                })
                .ToListAsync();

            var historialIds = datos.Select(h => h.Id).ToList();

            var difuntos = await _context.HistorialContratosConcesionDifuntos
                .Where(d => historialIds.Contains(d.HistorialContratoId))
                .Select(d => new
                {
                    d.HistorialContratoId,
                    Persona = new PersonaTablaGeneral
                    {
                        Nombre = d.Difunto.Nombre ?? "",
                        Apellido = d.Difunto.Apellido ?? ""
                    }
                })
                .ToListAsync();

            return datos.Select(h => new HistorialContratoTablaDTO
            {
                Id = h.Id,
                TramiteId = h.TramiteId,
                Concesion = h.Concesion,
                FechaContrato = h.FechaContrato,
                EsRenovacion = h.EsRenovacion,
                TipoParcelaId = h.TipoParcelaId ?? 0,
                NombreSeccion = h.NombreSeccion,
                NroParcela = h.NroParcela,
                NroFila = h.NroFila,
                Difuntos = difuntos.Where(d => d.HistorialContratoId == h.Id).Select(d => d.Persona).ToList()
            }).ToList();
        }
    }
}
