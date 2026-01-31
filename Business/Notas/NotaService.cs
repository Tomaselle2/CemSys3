using CemSys3.DTOs.HistorialEstado;
using CemSys3.DTOs.Nota;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.Tarea;
using CemSys3.DTOs.Tramite;
using CemSys3.Enumerables;
using CemSys3.Interfaces.HistorialEstados;
using CemSys3.Interfaces.Notas;
using CemSys3.Interfaces.Tarea;
using CemSys3.Interfaces.Tramite;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace CemSys3.Business.Notas
{
    public class NotaService : INotas
    {
        private readonly AppDbContext _context;
        private readonly ITarea _tareaService;
        private readonly IHistorialEstados _historialEstadoService;
        private readonly ITramite _tramiteService;
        public NotaService(AppDbContext context, ITarea tareaService, IHistorialEstados historialEstadosService, ITramite tramiteService)
        {
            _context = context;
            _tareaService = tareaService;
            _historialEstadoService = historialEstadosService;
            _tramiteService = tramiteService;
        }

        public async Task Add(NotaDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                //se registra el trámite
                TramiteDTO tramiteDTO = new TramiteDTO
                {
                    FechaCreacion = DateTime.Now,
                    TipoTramiteId = (int)TipoTramiteEnum.Nota,
                    UsuarioId = dto.UsurioId,
                    EstadoActualId = (int)EstadosNotaEnum.NotaPendiente,
                    Visibilidad = true
                };
                int tramiteId = await _tramiteService.Add(tramiteDTO);
                await _context.SaveChangesAsync(); //guardar el trámite para obtener el Id


                //se registra la nota
                Nota nuevaNota = new Nota
                {
                    TramiteId = tramiteId,
                    Nombre = dto.Nombre,
                    TipoNotaId = dto.TipoNotaId,
                    Descripcion = dto.Descripcion,
                    Color = dto.Color,
                    Visibilidad = true,
                };

                await _context.Notas.AddAsync(nuevaNota);
                await _context.SaveChangesAsync();

                //se registran las tareas asociadas a la nota

                if (dto.Tareas != null && dto.Tareas.Count > 0)
                {
                    foreach (var tareaDto in dto.Tareas)
                    {
                        TareaDTO nuevaTarea = new TareaDTO
                        {
                            NotaId = nuevaNota.TramiteId,
                            Estado = tareaDto.Estado,
                            Descripcion = tareaDto.Descripcion,
                            Visibilidad = true
                        };
                        await _tareaService.Add(nuevaTarea);
                    }
                }

                //se registra el historial de estados
                HistorialEstadosDTO dtoHistorial = new HistorialEstadosDTO
                {
                    Fecha = DateTime.Now,
                    TramiteId = tramiteId,
                    EstadoTramiteId = (int)EstadosNotaEnum.NotaPendiente
                };
                await _historialEstadoService.Add(dtoHistorial);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<NotaDTO> Get(int id)
        {
            NotaDTO notaDTO = new NotaDTO();

            //obtener la nota por id
            Nota nota = await _context.Notas.Include(t=>t.Tramite).Where(n=> n.TramiteId == id).FirstOrDefaultAsync() ?? throw new Exception("Nota no encontrada");

            //obtener todas las tareas asociadas a la nota
            IEnumerable<TareaDTO> tareas = await _tareaService.GetAllByNota(nota.TramiteId);

            //mapear la nota a NotaDTO
            notaDTO.Id = nota.TramiteId;
            notaDTO.Nombre = nota.Nombre;
            notaDTO.TipoNotaId = nota.TipoNotaId;
            notaDTO.Descripcion = nota.Descripcion;
            notaDTO.Color = nota.Color;
            notaDTO.Visibilidad = nota.Visibilidad;
            notaDTO.EstadoId = nota.Tramite.EstadoActualId;
            notaDTO.FechaCreacion = nota.Tramite.FechaCreacion;
            notaDTO.TramiteId = nota.TramiteId; //si esta asociada a un tramite
            notaDTO.Tareas = tareas.ToList();

            return notaDTO;
        }

        public async Task<PaginadoResponse<NotaDTO>> GetPaginadoByTipo(int estadoId, int filtroTipoNota = 0, int pagina = 1, int porPagina = 10)
        {
            PaginadoResponse<NotaDTO> resultado = new PaginadoResponse<NotaDTO>();

            // Filtro por estado de la nota
            var query = _context.Notas.Include(t=> t.Tramite).Where(n=> n.Tramite.EstadoActualId == estadoId);

            if(query != null)
            {
                // Filtro por tipo de nota
                switch (filtroTipoNota)
                {
                    case 1: //Ingreso
                        query = query.Where(e => e.TipoNotaId == (int)TipoNotaEnum.Ingreso);
                        break;
                    case 2: //Recordatorio
                        query = query.Where(e => e.TipoNotaId == (int)TipoNotaEnum.Recordatorio);
                        break;
                    case 0: //todos
                        break;
                    default:
                        // No aplicar filtro
                        break;
                }
            }
            
            // Total de registros
            var total = query != null ? await query.CountAsync() : 0;

            if(total > 0 && query != null)
            {
                // Paginación
                resultado.Paginacion.TotalPaginas = (int)Math.Ceiling(total / (double)porPagina);
                resultado.Paginacion.PaginaActual = Math.Max(1, Math.Min(pagina, resultado.Paginacion.TotalPaginas));
                resultado.Paginacion.RegistrosPorPagina = porPagina;
                resultado.Paginacion.Accion = "Index";
                resultado.Paginacion.Controlador = "Nota";
                resultado.Paginacion.TotalRegistros = total;

                // Obtener datos paginados - ORDENAR ANTES de la proyección
                resultado.Items = await query
                    .OrderByDescending(e => e.Tramite.FechaCreacion) // Ordenar por fecha antes de paginar
                    .Skip((resultado.Paginacion.PaginaActual - 1) * porPagina)
                    .Take(porPagina)
                    .Select(s => new NotaDTO
                    {
                        Id = s.TramiteId,
                        Nombre = s.Nombre,
                        TipoNotaId = s.TipoNotaId,
                        Descripcion = s.Descripcion,
                        Color = s.Color,
                        Visibilidad = s.Visibilidad,
                        EstadoId = s.Tramite.EstadoActualId,
                        FechaCreacion = s.Tramite.FechaCreacion // Incluir la fecha
                    })
                    .ToListAsync();
            }else
            {
                resultado.Items = new List<NotaDTO>();
            }

            return resultado;
        }

        public async Task Update(NotaDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Models.Nota nota = await _context.Notas
                    .Include(n => n.Tramite)
                    .FirstOrDefaultAsync(n => n.TramiteId == dto.Id)
                    ?? throw new Exception("Nota no encontrada");

                nota.Nombre = dto.Nombre;
                nota.Descripcion = dto.Descripcion;
                nota.Color = dto.Color;
                nota.Tramite.EstadoActualId = dto.EstadoId;

                //se registra el historial de estados
                if (nota.Tramite.EstadoActualId == (int)EstadosNotaEnum.NotaFinalizado)
                {
                    HistorialEstadosDTO dtoHistorial = new HistorialEstadosDTO
                    {
                        Fecha = DateTime.Now,
                        TramiteId = nota.TramiteId,
                        EstadoTramiteId = (int)EstadosNotaEnum.NotaFinalizado
                    };
                    await _historialEstadoService.Add(dtoHistorial);

                    //se actualiza el estado actual del trámite
                    TramiteDTO tramiteActualizar = new TramiteDTO
                    {
                        Id = nota.TramiteId,
                        EstadoActualId = (int)EstadosNotaEnum.NotaFinalizado
                    };

                    await _tramiteService.Update(tramiteActualizar);
                }

                foreach (var tareaDto in dto.Tareas)
                {
                    // ELIMINAR
                    if (tareaDto.Eliminada && tareaDto.Id > 0)
                    {
                        await _tareaService.Delete(tareaDto.Id);
                        continue;
                    }

                    // ACTUALIZAR
                    if (tareaDto.Id > 0)
                    {
                        await _tareaService.Update(new TareaDTO
                        {
                            Id = tareaDto.Id,
                            Descripcion = tareaDto.Descripcion,
                            Estado = tareaDto.Estado
                        });
                    }
                    // AGREGAR
                    else
                    {
                        await _tareaService.Add(new TareaDTO
                        {
                            NotaId = dto.Id,
                            Descripcion = tareaDto.Descripcion,
                            Estado = tareaDto.Estado,
                            Visibilidad = true
                        });
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
