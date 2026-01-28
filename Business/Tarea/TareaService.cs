using CemSys3.DTOs.Tarea;
using CemSys3.Interfaces.Tarea;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Business.Tarea
{
    public class TareaService : ITarea
    {
        private readonly AppDbContext _context;

        public TareaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(TareaDTO dto)
        {
            await _context.Tareas.AddAsync(new Models.Tarea
            {
                Estado = dto.Estado,
                Descripcion = dto.Descripcion,
                NotaId = dto.NotaId,
                TramiteId = dto.TramiteId,
                Visibilidad = dto.Visibilidad
            });

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            CemSys3.Models.Tarea? tarea = await _context.Tareas.FindAsync(id);

            if (tarea == null)
            {
                throw new Exception("No se encontro la tarea id: " + id.ToString());
            }

            _context.Tareas.Remove(tarea);
        }

        public async Task<IEnumerable<TareaDTO>> GetAllByNota(int notaId)
        {
            return await _context.Tareas
                .Where(t => t.NotaId == notaId)
                .Select(t => new TareaDTO
                {
                    Id = t.Id,
                    Estado = t.Estado,
                    Descripcion = t.Descripcion,
                    NotaId = t.NotaId,
                    TramiteId = t.TramiteId,
                    Visibilidad = t.Visibilidad
                }).ToListAsync();
        }

        public async Task<IEnumerable<TareaDTO>> GetAllByTramite(int tramiteId)
        {
            return await _context.Tareas
                            .Where(t => t.TramiteId == tramiteId)
                            .Select(t => new TareaDTO
                            {
                                Id = t.Id,
                                Estado = t.Estado,
                                Descripcion = t.Descripcion,
                                NotaId = t.NotaId,
                                TramiteId = t.TramiteId,
                                Visibilidad = t.Visibilidad
                            }).ToListAsync();
        }

        public async Task Update(TareaDTO dto)
        {
            CemSys3.Models.Tarea tarea = await _context.Tareas.FindAsync(dto.Id) ?? throw new Exception("No se encontro la tarea id: " + dto.Id.ToString());
            tarea.Estado = dto.Estado;
            tarea.Descripcion = dto.Descripcion;
        }

    }
}
