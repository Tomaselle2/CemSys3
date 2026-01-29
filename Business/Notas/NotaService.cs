using CemSys3.DTOs.Nota;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.Tarea;
using CemSys3.Enumerables;
using CemSys3.Interfaces.Notas;
using CemSys3.Interfaces.Tarea;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace CemSys3.Business.Notas
{
    public class NotaService : INotas
    {
        private readonly AppDbContext _context;
        private readonly ITarea _tareaService;
        public NotaService(AppDbContext context, ITarea tareaService)
        {
            _context = context;
            _tareaService = tareaService;
        }

        public async Task Add(NotaDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                //se registra la nota
                Nota nuevaNota = new Nota
                {
                    Nombre = dto.Nombre,
                    TipoNotaId = dto.TipoNotaId,
                    Descripcion = dto.Descripcion,
                    Color = dto.Color,
                    Visibilidad = dto.Visibilidad,
                    FechaCreacion = DateTime.Now,
                    EstadoId = (int)EstadosNotaEnum.NotaPendiente
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
                            NotaId = nuevaNota.Id,
                            Estado = tareaDto.Estado,
                            Descripcion = tareaDto.Descripcion,
                            Visibilidad = true
                        };
                        await _tareaService.Add(nuevaTarea);
                    }
                }

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
            Nota nota = await _context.Notas.FindAsync(id) ?? throw new Exception("Nota no encontrada");

            //obtener todas las tareas asociadas a la nota
            IEnumerable<TareaDTO> tareas = await _tareaService.GetAllByNota(nota.Id);

            //mapear la nota a NotaDTO
            notaDTO.Id = nota.Id;
            notaDTO.Nombre = nota.Nombre;
            notaDTO.TipoNotaId = nota.TipoNotaId;
            notaDTO.Descripcion = nota.Descripcion;
            notaDTO.Color = nota.Color;
            notaDTO.Visibilidad = nota.Visibilidad;
            notaDTO.EstadoId = nota.EstadoId;
            notaDTO.FechaCreacion = nota.FechaCreacion;
            notaDTO.TramiteId = nota.TramiteId;
            notaDTO.Tareas = tareas.ToList();

            return notaDTO;
        }

        public async Task<PaginadoResponse<NotaDTO>> GetPaginadoByTipo(int estadoId, int filtroTipoNota = 0, int pagina = 1, int porPagina = 10)
        {
            PaginadoResponse<NotaDTO> resultado = new PaginadoResponse<NotaDTO>();

            // Filtro por estado de la nota
            var query = _context.Notas.Where(n=> n.EstadoId == estadoId);

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
                    .OrderByDescending(e => e.FechaCreacion) // Ordenar por fecha antes de paginar
                    .Skip((resultado.Paginacion.PaginaActual - 1) * porPagina)
                    .Take(porPagina)
                    .Select(s => new NotaDTO
                    {
                        Id = s.Id,
                        Nombre = s.Nombre,
                        TipoNotaId = s.TipoNotaId,
                        Descripcion = s.Descripcion,
                        Color = s.Color,
                        Visibilidad = s.Visibilidad,
                        EstadoId = s.EstadoId,
                        FechaCreacion = s.FechaCreacion // Incluir la fecha
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
            using var Transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Nota nota = await _context.Notas.FindAsync(dto.Id) ?? throw new Exception("Nota no encontrada");
                nota.Nombre = dto.Nombre;
                nota.Descripcion = dto.Descripcion;
                nota.Color = dto.Color;

                IEnumerable<TareaDTO> tareasExistentes = await _tareaService.GetAllByNota(nota.Id);

                // Actualizar tareas existentes y agregar nuevas tareas
                foreach (var tareaDto in dto.Tareas)
                {
                    var tareaExistente = tareasExistentes.FirstOrDefault(t => t.Id == tareaDto.Id);

                    if (tareaExistente != null)
                    {
                        // Actualizar tarea existente
                        tareaExistente.Descripcion = tareaDto.Descripcion;
                        tareaExistente.Estado = tareaDto.Estado;
                        await _tareaService.Update(tareaExistente);
                    }
                    else
                    {
                        // Agregar nueva tarea
                        TareaDTO nuevaTarea = new TareaDTO
                        {
                            NotaId = nota.Id,
                            Descripcion = tareaDto.Descripcion,
                            Estado = tareaDto.Estado,
                            Visibilidad = true
                        };
                        await _tareaService.Add(nuevaTarea);
                    }
                }
                
                await _context.SaveChangesAsync();
                await Transaction.CommitAsync();
            }
            catch (Exception)
            {
                await Transaction.RollbackAsync();
                throw;
            }
        }
    }
}
