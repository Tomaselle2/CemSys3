using CemSys3.DTOs.Arbol;
using CemSys3.Interfaces;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Business.Arbol
{
    public class ArbolService : IArbol
    {
        private readonly AppDbContext _context;

        public ArbolService(AppDbContext context)
        {
            _context = context;
        }
        public async Task GuardarDiagrama(ArbolDTO dto)
        {
            var entidad = await _context.Diagramas.FirstOrDefaultAsync(x => x.TramiteId == dto.TramiteId);

            if (entidad == null)
            {
                entidad = new Diagrama
                {
                    TramiteId = dto.TramiteId,
                    JsonDiagrama = dto.JsonDiagrama,
                    FechaCreacion = DateTime.Now
                };

                _context.Add(entidad);
            }
            else
            {
                entidad.JsonDiagrama = dto.JsonDiagrama;
                entidad.FechaModificacion = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<ArbolDTO> ObtenerDiagrama(int tramiteId)
        {
            var diagrama = await _context.Diagramas
                .FirstOrDefaultAsync(x => x.TramiteId == tramiteId);

            return new ArbolDTO
            {
                TramiteId = diagrama?.TramiteId ?? 0,
                JsonDiagrama = diagrama?.JsonDiagrama ?? "",
                FechaCreacion = diagrama?.FechaCreacion ?? DateTime.Now,
                FechaModificacion = diagrama?.FechaModificacion
            };
        }
    }
}
