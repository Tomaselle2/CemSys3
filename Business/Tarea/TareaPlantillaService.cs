using CemSys3.DTOs.Tarea;
using CemSys3.Interfaces.Tarea;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Business.Tarea
{
    public class TareaPlantillaService : ITareaPlantilla
    {

        private readonly AppDbContext _context;
        public TareaPlantillaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task GuardarTareas(int tipoTramiteId, List<TareaDTO> tareas)
        {
            var existentes = await _context.TareaPlantillas
                .Where(t => t.TipoTramiteId == tipoTramiteId)
                .ToListAsync();

            foreach (var tarea in tareas)
            {
                // 🟢 NUEVA
                if (tarea.Id == 0 && !tarea.Eliminada)
                {
                    await _context.TareaPlantillas.AddAsync(new Models.TareaPlantilla
                    {
                        Descripcion = tarea.Descripcion,
                        Estado = false,
                        TipoTramiteId = tipoTramiteId,
                        Visibilidad = true
                    });
                }
                else
                {
                    var existente = existentes.FirstOrDefault(t => t.Id == tarea.Id);
                    if (existente == null) continue;

                    // 🔴 ELIMINADA
                    if (tarea.Eliminada)
                    {
                        _context.TareaPlantillas.Remove(existente);
                    }
                    // 🟡 UPDATE
                    else
                    {
                        existente.Descripcion = tarea.Descripcion;
                        existente.Estado = tarea.Estado;
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task CrearTareasPorTramite(int tramiteId, int tipoTramiteId)
        {
          
            IEnumerable<Models.TareaPlantilla> plantillas = await _context.TareaPlantillas
            .Where(t => t.TipoTramiteId == tipoTramiteId).ToListAsync();

                IEnumerable<Models.Tarea> tareas = plantillas.Select(p => new Models.Tarea
                {
                    Estado = false,
                    Descripcion = p.Descripcion,
                    TramiteId = tramiteId,
                    Visibilidad = p.Visibilidad,
                    TareaPlantillaId = p.Id
                });

                await _context.Tareas.AddRangeAsync(tareas);
                await _context.SaveChangesAsync();
        }

        public async Task<List<TareaDTO>> GetAllByTipoTramite(int tipoTramiteId)
        {
            return await _context.TareaPlantillas.Where(t => t.TipoTramiteId == tipoTramiteId)
                .Select(t => new TareaDTO
                {
                    Id = t.Id,
                    Estado = t.Estado,
                    Descripcion = t.Descripcion,
                    Visibilidad = t.Visibilidad,
                    TipoTramiteId = t.TipoTramiteId
                }).ToListAsync();
        }

    }
}
